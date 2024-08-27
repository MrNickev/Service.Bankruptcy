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

namespace Application.Fedresurs.Implementations;

/// <summary>
/// Сервис авторизации в API Федресурса
/// </summary>
/// <param name="cache">Кэш в котором сохраняется токен</param>
/// <param name="configuration">Конфигурация Федресурса</param>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
public class AuthService(IMemoryCache cache, FedresursConfiguration configuration, HttpClient httpClient, ILogger<AuthService> logger)
    : IAuthService
{
    /// <inheritdoc/>>
    public async Task<string> GetToken()
    {
        if (!cache.TryGetValue("fedresursJwtToken", out string token))
        {
            httpClient.BaseAddress = new Uri(configuration.Host);
            var response = await httpClient.PostAsync("/v1/auth", new StringContent(JsonConvert.SerializeObject(new AuthRequest(configuration.Login, configuration.Password)), Encoding.UTF8, "application/json" ));
            
            if (!response.IsSuccessStatusCode)
            {
                throw new RequestException((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            
            var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();
            logger.LogInformation($"Send request to {configuration.Host}. Get data: ${response.Content}");
            
            if (responseContent != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(3));
                cache.Set("fedresursJwtToken", responseContent.JwtToken, cacheOptions);
                token = responseContent.JwtToken;
            }
        }
        logger.LogInformation($"Using token: {token}");
        return token;
    }
}