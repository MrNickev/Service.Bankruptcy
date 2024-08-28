using System.Net.Http.Json;
using System.Text;
using Application.Common.Exceptions;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Configuration;
using Application.Fedresurs.Models;
using Application.Fedresurs.Models.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Fedresurs.Services;

/// <summary>
/// Сервис авторизации в API Федресурса
/// </summary>

public class AuthService : IAuthService
{
    private readonly IMemoryCache _cache;
    private readonly FedresursConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Конструктор AuthService
    /// </summary>
    /// <param name="cache">Кэш в котором сохраняется токен</param>
    /// <param name="configuration">Конфигурация Федресурса</param>
    /// <param name="httpClientFactory"></param>
    /// <param name="logger"></param>
    public AuthService(IMemoryCache cache, FedresursConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AuthService> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("fedresursClient");
    }


    /// <inheritdoc/>>
    public async Task<string> GetToken()
    {
        if (!_cache.TryGetValue("fedresursJwtToken", out string token))
        {
            var response = await _httpClient.PostAsync("/v1/auth", new StringContent(JsonConvert.SerializeObject(new AuthRequest(_configuration.Login, _configuration.Password)), Encoding.UTF8, "application/json" ));
            
            if (!response.IsSuccessStatusCode)
            {
                throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            
            var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();
            _logger.LogInformation($"Send request to {_configuration.Host}. Get data: ${response.Content}");
            
            if (responseContent != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(3));
                _cache.Set("fedresursJwtToken", responseContent.JwtToken, cacheOptions);
                token = responseContent.JwtToken;
            }
        }
        _logger.LogInformation($"Using token: {token}");
        return token;
    }
}