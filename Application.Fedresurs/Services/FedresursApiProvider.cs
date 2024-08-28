using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.RateLimiting;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Configuration;
using Application.Fedresurs.Models;

namespace Application.Fedresurs.Services;

public class FedresursApiProvider : IApiProvider
{
    //TODO: вынести это в конфиг, подробнее: https://thecodeman.net/posts/how-to-implement-rate-limiter-in-csharp
    private readonly FedresursConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public FedresursApiProvider(IAuthService authService, FedresursConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("fedresursClient");
        var token = authService.GetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
    }
    
    /// <summary>
    /// Поиск банкрота. Для корректного поиска должны быть заполнены ФИО/ИНН/СНИЛС. 
    /// </summary>
    /// <param name="bankrupt">Объект параметров банкрота</param>
    /// <returns>Список банкоротов (в идеале 1), найденных по входным параметрам</returns>
    public async Task<Bankrupt<Person>?> FindBankrupt(Bankrupt bankrupt, CancellationToken cancellationToken = default)
    {
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
        
        var response = await _httpClient.GetAsync($"v1/bankrupts?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}&limit=10&offset=0", cancellationToken);
        
        if (!response.IsSuccessStatusCode)
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
        var response = await _httpClient.GetAsync($"v1/messages?" +
                                                  $"bankruptGUID={bankruptGuid}&" +
                                                  $"sort=DatePublish:asc&" +
                                                  $"limit={limit}&" +
                                                  $"offset={offset}&" +
                                                  //ищем только сообщения-маркеры на какой стадии банкротство
                                                  $"courtDecisionType={string.Join(",", courtDecisionTypes)}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Message>>();
        return responseContent;
        
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

        var response = await _httpClient.GetAsync($"v1/reports?" +
                                                  $"bankruptGUID={bankruptGuid}&" +
                                                  $"sort=DatePublish:asc&" +
                                                  $"limit={limit}&" +
                                                  $"offset={offset}&" +
                                                  //ищем только сообщения-маркеры на какой стадии банкротство
                                                  $"procedureType={string.Join(",", procedureTypes)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
        }
        
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Report>>(cancellationToken: cancellationToken);
        return responseContent;
    }

    
    
    
}