using Application.Common.Exceptions;
using Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Application.Common;

public static class ConfigurationRegisterServices
{
    private const string AppsettingsName = "appsettings.json";

    public static void RegisterConfiguration<TModel>(this IServiceCollection services) where TModel : class
    {
        services.AddScoped<TModel>(_ => RegisterConfiguration<TModel>());
    }

    public static TModel RegisterConfiguration<TModel>() where TModel : class
    {
        var configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppsettingsName);

        if (File.Exists(configurationPath) is false)
        {
            throw new ConfigurationException("Не найден файл конфигурации для импорта в настройки приложения");
        }

        var configurationText = File.ReadAllText(configurationPath);

        var configurationModel = JsonConvert.DeserializeObject<ConfigurationBase<TModel>>(configurationText);

        if (configurationModel?.Model is null)
        {
            throw new ConfigurationException($"Не удалось десериализовать конфигурацию - {typeof(TModel).Name}");
        }
        
        return configurationModel.Model;
    }
}