using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Common.Astractions;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Models;
using Application.Fedresurs.Models.Configuration;
using System.Threading.RateLimiting;

namespace Application.Fedresurs.Implementations;

/// <summary>
/// Сервис проверки на банкротство через API Федресурса
/// </summary>
public class FedresursBankruptcyCheckService : IBankruptcyCheckService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly FedresursConfiguration _configuration;
    private readonly RateLimiter _rateLimiter;

    public FedresursBankruptcyCheckService(HttpClient httpClient, IAuthService authService, FedresursConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
        _authService = authService;
        
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_configuration.Host);
        var token = _authService.GetToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
        
        _rateLimiter = new SlidingWindowRateLimiter(
            new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 8,
                Window = TimeSpan.FromSeconds(1),
                SegmentsPerWindow = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            });
    }


    /// <inheritdoc /> 
    public async Task<ClientBankruptcyCheckResult> Check(BankruptcyCheckRequest request)
    {
        //сначала делаем поиск по всем данным которые есть, если ничего не нашли, то пробуем искать отдельно по ИНН и по СНИЛС
        //имена в нашей базе и базе федресурса могут различаться, поэтому лучше искать по ИНН или СНИЛС - что-то из этого еть всегда
        var bankrupt = await FindBankrupt(request) ?? 
                       await FindBankrupt(new BankruptcyCheckRequest() {Type = request.Type, Inn = request.Inn}) ?? 
                       await FindBankrupt(new BankruptcyCheckRequest() {Type = request.Type, Snils = request.Snils});

        if (bankrupt is null)
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.NotBankrupt, GetType().Name);
        }
            
        var finishedBankruptcyMessages = await FindMessages(bankrupt.Guid, new []
        {
            "LegalCaseEnd",
            "PropertySaleComplete"
        });
        
        
        //проверяем есть ли сообщения об окончании банкротства и актуальны ли они
        if (finishedBankruptcyMessages.Total > 0 && finishedBankruptcyMessages.Data.Any(m => m.DatePublish > DateTime.Today.AddYears(-3)))
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.FinishedBankruptcy, GetType().Name);
        }

        
        var refusalBankruptcyMessages = await FindMessages(bankrupt.Guid, new[]
        {
            "BankruptcyRefusal",
            "ObligationsDischargeRefusal"
        });
        
        if (refusalBankruptcyMessages.Total > 0 && refusalBankruptcyMessages.Data.Any(m => m.DatePublish > DateTime.Today.AddYears(-3)))
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.RefusalBankruptcy, GetType().Name);
        }
        

        var proceduralBankruptcyMessages = await FindMessages(bankrupt.Guid, new[]
        {
            "Receivership",
            "DebtRestructuring",
            "PropertySale",
        });

        var proceduralBankruptcyReports = await FindReports(bankrupt.Guid, new[]
        {
            "Tender",
            "Watching",
            "CitizenAssetsDisposal",
            "CitizenDebtRestructuring"
        });

        if (proceduralBankruptcyMessages.Total > 0 || proceduralBankruptcyReports.Total > 0)
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.ProceduralBankruptcy, GetType().Name);
        }

        return new ClientBankruptcyCheckResult(request, BankruptStatus.NotBankrupt, GetType().Name);
    }


    /// <summary>
    /// Поиск банкрота 
    /// </summary>
    /// <param name="bankrupt">Объект параметров банкрота</param>
    /// <returns>Список банкоротов (в идеале 1), найденных по входным параметрам</returns>
    private async Task<Bankrupt<Person>?> FindBankrupt(Bankrupt bankrupt)
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

        var requestUri =
            $"v1/bankrupts?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}&limit=10&offset=0";

        if (queryParams.Keys.Count < 2)
            return null;
        
        var response =
             await _httpClient.GetAsync(
                $"v1/bankrupts?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}&limit=10&offset=0");

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

    // private async Task<Bankrupt<Person>?> FindBankrupt(string? name = null, string? inn = null, string? snils = null)
    // {
    //     var queryParams = new Dictionary<string, string>();
    //     
    //     var response =
    //         await _httpClient.GetAsync(
    //             $"v1/bankrupts?{name ?? "name=" + name}&limit=10&offset=0");
    // }


    /// <summary>
    /// Найти сообщения по банкротам
    /// </summary>
    /// <param name="bankruptGuid">Guid банкрота для которого ведется поиск сообщений</param>
    /// <param name="courtDecisionTypes">Тип судебного акта</param>
    /// <param name="limit">Лимит возвращаемых записей</param>
    /// <param name="offset">Сдвиг по поиску</param>
    /// <returns>Объект <see cref="PageData{Message}"/> со списком сообщений</returns>
    private async Task<PageData<Message>> FindMessages(Guid bankruptGuid, string[] courtDecisionTypes, int limit = 500, int offset = 0)
    {
        var response =
            await _httpClient.GetAsync(
                $"v1/messages?" +
                $"bankruptGUID={bankruptGuid}&" +
                $"sort=DatePublish:asc&" +
                $"limit={limit}&" +
                $"offset={offset}&" +
                //ищем только сообщения-маркеры на какой стадии банкротство
                $"courtDecisionType={string.Join(",", courtDecisionTypes)}");

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
    private async Task<PageData<Report>> FindReports(Guid bankruptGuid, string[] procedureTypes, int limit = 500, int offset = 0)
    {
        var response =
            await _httpClient.GetAsync(
                $"v1/reports?" +
                $"bankruptGUID={bankruptGuid}&" +
                $"sort=DatePublish:asc&" +
                $"limit={limit}&" +
                $"offset={offset}&" +
                //ищем только сообщения-маркеры на какой стадии банкротство
                $"procedureType={string.Join(",", procedureTypes)}");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Report>>();
        return responseContent;
    }
}