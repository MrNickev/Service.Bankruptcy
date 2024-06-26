using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Common.Astractions;
using Application.Common.Models;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Models;
using Application.Fedresurs.Models.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Fedresurs.Implementations;

/// <summary>
/// Сервис проверки на банкротство через Федресурс
/// </summary>
public class FedresursBankruptcyCheckService : IBankruptcyCheckService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly FedresursConfiguration _configuration;

    public FedresursBankruptcyCheckService(HttpClient httpClient, IAuthService authService, FedresursConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
        _authService = authService;
        
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_configuration.Host);
    }


    /// <inheritdoc /> 
    public async Task<BankruptCheckResult> Check(BankruptcyCheckRequest request)
    {
        var token = await _authService.GetToken();
        _httpClient.BaseAddress = new Uri(_configuration.Host);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var bankrupts = await FindBankrupts(request);
        
        if (bankrupts.Total == 0)
        {
            return new BankruptCheckResult(BankruptStatus.NotBankrupt, this.GetType().Name);
        }
        
        if (bankrupts.Total > 1)
        {
            throw new Exception("Найдено слишком много банкротов с такимми данныи");
        }
        
        var bankrupt = bankrupts.Data[0];
        var messages = await FindMessages(bankrupt.Guid);
        if (messages.Total == 0)
        {
            return new BankruptCheckResult(BankruptStatus.PossibleBankruptcy, this.GetType().Name); 
        }

        var reports = await FindReports(bankrupt.Guid);
        if (reports.Total == 0)
        {
            return new BankruptCheckResult(BankruptStatus.ProceduralBankruptcy, this.GetType().Name);
        }
        return new BankruptCheckResult(BankruptStatus.FinishedBankruptcy, this.GetType().Name);
    }

    /// <summary>
    /// Поиск банкрота 
    /// </summary>
    /// <param name="request">Объект параметров банкрота</param>
    /// <returns>Список банкоротов (в идеале 1), найденных по входным параметрам</returns>
    private async Task<PageData<Bankrupt<Person>>> FindBankrupts(BankruptcyCheckRequest request)
    {
        if (request.Limit == 0)
            request.Limit = 50;

        var queryParams = new Dictionary<string, string>();

        var properties = request.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(request);
            if (value != null)
            {
                queryParams.Add(prop.Name.ToLower(), value.ToString());
            }
        }

        var path =
            $"v1/bankrupts?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";
        var response =
             await _httpClient.GetAsync(
                $"v1/bankrupts?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}");

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Bankrupt<Person>>>();

        return responseContent;
    }

    
    /// <summary>
    /// Найти сообщения по банкрота
    /// </summary>
    /// <param name="bankruptGuid">Guid банкрота для которого ведется поиск сообщений</param>
    /// <returns>Объект <see cref="PageData{Message}"/> со списком сообщений</returns>
    private async Task<PageData<Message>> FindMessages(Guid bankruptGuid, int limit = 500, int offset = 0)
    {
        var response =
            await _httpClient.GetAsync(
                $"v1/messages?" +
                $"bankruptGUID={bankruptGuid}&" +
                $"sort=DatePublish:asc&" +
                $"limit={limit}&" +
                $"offset={offset}");

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Message>>();
        return responseContent;
    }

    
    /// <summary>
    /// Найти отчеты по банкроту
    /// </summary>
    /// <param name="bankruptGuid"></param>
    /// <returns></returns>
    private async Task<PageData<Report>> FindReports(Guid bankruptGuid, int limit = 500, int offset = 0)
    {
        var response =
            await _httpClient.GetAsync(
                $"v1/reports?bankruptGUID={bankruptGuid}" +
                $"&limit={limit}" +
                $"&offset={offset}");

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<PageData<Report>>();
        return responseContent;
    }
}