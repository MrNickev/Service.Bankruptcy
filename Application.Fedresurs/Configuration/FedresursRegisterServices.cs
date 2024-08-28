using System.Threading.RateLimiting;
using Application.Common.Astractions;
using Application.Common.Handlers;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Services;
using InfrastructureLayer.Configuration;
using InfrastructureLayer.Configuration.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Fedresurs.Configuration;

public class FedresursRegisterServices : IRegisterService
{
    /// <summary>
    /// Регистрация сервисов модуля Федресурс'а
    /// </summary>
    /// <param name="services"></param>
    public void Register(IServiceCollection services)
    {
        services.RegisterConfiguration<FedresursConfiguration>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IApiProvider, FedresursApiProvider>();
        services.AddScoped<IBankruptcyCheckService, FedresursBankruptcyCheckService>();
        
        
        var fedresursConfig = ConfigurationRegister.RegisterConfiguration<FedresursConfiguration>();
        services.AddSingleton(new ClientSideLimitedHandler(new SlidingWindowRateLimiter(
            new SlidingWindowRateLimiterOptions
            {
                PermitLimit = fedresursConfig.RequestsLimitPerSecond,
                Window = TimeSpan.FromSeconds(1),
                SegmentsPerWindow = 1,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 50
            }
        )));
        
        services.AddHttpClient("fedresursClient", c =>
            {
                c.BaseAddress = new Uri(fedresursConfig.Host);
            })
            .ConfigurePrimaryHttpMessageHandler<ClientSideLimitedHandler>();
    }
}