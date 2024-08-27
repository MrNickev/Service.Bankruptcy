using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.RateLimiting;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Configuration;
using Application.Fedresurs.Models;

namespace Application.Fedresurs.Implementations;

public class FedresursApiProvider
    : DelegatingHandler, IApiProvider, IAsyncDisposable
{
    //TODO: вынести это в конфиг, подробнее: https://thecodeman.net/posts/how-to-implement-rate-limiter-in-csharp
    private readonly RateLimiter _rateLimiter;
    private readonly IAuthService _authService;
    private readonly FedresursConfiguration _configuration;

    public FedresursApiProvider(IAuthService authService, FedresursConfiguration configuration) : base(new HttpClientHandler())
    {
        _authService = authService;
        _configuration = configuration;
        _rateLimiter = new SlidingWindowRateLimiter(
            new SlidingWindowRateLimiterOptions
            {
                PermitLimit = _configuration.RequestsLimitPerSecond,
                Window = TimeSpan.FromSeconds(1),
                SegmentsPerWindow = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 50
            }
        );
    }
    
    /// <summary>
    /// Поиск банкрота 
    /// </summary>
    /// <param name="bankrupt">Объект параметров банкрота</param>
    /// <returns>Список банкоротов (в идеале 1), найденных по входным параметрам</returns>
    public async Task<Bankrupt<Person>?> FindBankrupt(Bankrupt bankrupt, CancellationToken cancellationToken = default)
    {
        using var lease = await _rateLimiter.AcquireAsync(cancellationToken: cancellationToken);
        
        var queryParams = new Dictionary<string, string>();

        var properties = bankrupt.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(bankrupt);
            if (value != default)
            {
                queryParams.Add(prop.Name.ToLower(), value.ToString());
            }
        }

        if (queryParams.Keys.Count < 2)
            return null;

        HttpResponseMessage response = null;
        if (lease.IsAcquired)
        {
            var requestMessage = CreateRequest(HttpMethod.Get,
                    $"v1/bankrupts?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}&limit=10&offset=0");

            response = await base.SendAsync(requestMessage, cancellationToken);
        }
        
        if (response is null || !response.IsSuccessStatusCode)
        {
            throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Bankrupt<Person>>>();
        
        if (responseContent.Total == 0)
        {
            return null;
        }
        
        if (responseContent.Total > 1)
        {
            throw new Exception("Найдено несколько людей с такими данными. Уточните запрос используя ИНН..");
        }

        return responseContent.Data[0];
    }

    /// <summary>
    /// Найти сообщения по банкротам
    /// </summary>
    /// <param name="bankruptGuid">Guid банкрота для которого ведется поиск сообщений</param>
    /// <param name="courtDecisionTypes">Тип судебного акта</param>
    /// <param name="limit">Лимит возвращаемых записей</param>
    /// <param name="offset">Сдвиг по поиску</param>
    /// <returns>Объект <see cref="PageData{Message}"/> со списком сообщений</returns>
    public async Task<PageData<Message>> FindMessages(Guid bankruptGuid, string[] courtDecisionTypes, int limit = 500, int offset = 0, CancellationToken cancellationToken = default)
    {
        using var lease = await _rateLimiter.AcquireAsync(cancellationToken: cancellationToken);
        HttpResponseMessage response = null;
        if (lease.IsAcquired)
        {
            var requestMessage = CreateRequest(HttpMethod.Get,
                    $"v1/messages?" +
                    $"bankruptGUID={bankruptGuid}&" +
                    $"sort=DatePublish:asc&" +
                    $"limit={limit}&" +
                    $"offset={offset}&" +
                    //ищем только сообщения-маркеры на какой стадии банкротство
                    $"courtDecisionType={string.Join(",", courtDecisionTypes)}");

            response = await base.SendAsync(requestMessage, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            var responseContent = await response.Content.ReadFromJsonAsync<PageData<Message>>();
            return responseContent;
        }

        
        return null;
    }

    /// <summary>
    /// Найти отчеты по банкроту
    /// </summary>
    /// <param name="bankruptGuid">GUID банкрота</param>
    /// <param name="procedureTypes">Тип отчетов</param>
    /// <param name="limit"></param>
    /// <param name="offset"></param>
    /// <returns>Список отчетов в формате <see cref="PageData{Report}"/></returns>
    /// <exception cref="RequestException"></exception>
    public async Task<PageData<Report>> FindReports(Guid bankruptGuid, string[] procedureTypes, int limit = 500, int offset = 0, CancellationToken cancellationToken = default)
    {
        using var lease = await _rateLimiter.AcquireAsync(cancellationToken: cancellationToken);
        HttpResponseMessage response = null;
        if (lease.IsAcquired)
        {
            var requestMessage = CreateRequest(HttpMethod.Get,
                    $"v1/reports?" +
                    $"bankruptGUID={bankruptGuid}&" +
                    $"sort=DatePublish:asc&" +
                    $"limit={limit}&" +
                    $"offset={offset}&" +
                    //ищем только сообщения-маркеры на какой стадии банкротство
                    $"procedureType={string.Join(",", procedureTypes)}");

            response = await base.SendAsync(requestMessage, cancellationToken);
        }

        if (response is null || !response.IsSuccessStatusCode)
        {
            throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Report>>();
        return responseContent;
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string address)
    {
        var requestMessage = new HttpRequestMessage(method, new Uri($"{_configuration.Host}/{address}"));
        var token = _authService.GetToken();
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
        return requestMessage;
    }
    
    async ValueTask IAsyncDisposable.DisposeAsync()
    { 
        await _rateLimiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _rateLimiter.Dispose();
        }
    }
}