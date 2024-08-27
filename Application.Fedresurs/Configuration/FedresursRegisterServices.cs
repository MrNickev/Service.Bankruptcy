using Application.Common.Astractions;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Implementations;
using Application.Fedresurs.Models.Configuration;
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
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IApiProvider, FedresursApiProvider>();
        services.AddScoped<IBankruptcyCheckService, FedresursBankruptcyCheckService>();
    }
}