using System.Net.Http.Json;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Models;
using Application.Fedresurs.Models.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Fedresurs.Implementations;

public class AuthService(HttpClient httpClient, IMemoryCache cache, FedresursConfiguration configuration)
    : IAuthService
{
    /// <inheritdoc/>>
    public async Task<string> GetToken()
    {
        if (cache.TryGetValue("fedresursJwtToken", out string token))
        {
            var response = await httpClient.PostAsJsonAsync(configuration.Host, new AuthRequest(configuration.Login, configuration.Password));
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            if (responseContent != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(3));
                cache.Set("fedresursJwtToken", responseContent.JwtToken, cacheOptions);
            }
        }

        return token;
    }
}