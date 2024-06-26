using Application.Common;
using Application.Common.Astractions;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Implementations;
using Application.Fedresurs.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Application.Fedresurs;

public static class FedresursRegisterServices
{
    /// <summary>
    /// Регистрация сервисов модуля Федресурс'а
    /// </summary>
    /// <param name="services"></param>
    public static void Register(IServiceCollection services)
    {
        services.RegisterConfiguration<FedresursConfiguration>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBankruptcyCheckService, FedresursBankruptcyCheckService>();
    }
}