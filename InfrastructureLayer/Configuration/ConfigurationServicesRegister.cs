using System.Reflection;
using System.Runtime.Loader;
using InfrastructureLayer.Configuration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace InfrastructureLayer.Configuration;

public static class ConfigurationServicesRegister
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
    
    public static void RegisterServices(this IServiceCollection services)
    {
        // Получаем все сборки, уже загруженные в домен приложения
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        // Получаем путь к директории приложения
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        // Ищем все файлы с расширением .dll и пытаемся их загрузить
        var dllFiles = Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories);
        foreach (var dll in dllFiles)
        {
            try
            {
                var assemblyName = AssemblyLoadContext.GetAssemblyName(dll);
                if (assemblies.All(a => a.GetName().Name != assemblyName.Name))
                {
                    assemblies.Add(Assembly.Load(assemblyName));
                }
            }
            catch
            {
                // Игнорируем сборки, которые не удается загрузить
            }
        }

        var registerServicesTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IRegisterService).IsAssignableFrom(type) &&
                           type is { IsClass: true, IsAbstract: false });

        foreach (var type in registerServicesTypes)
        {
            var instance = (IRegisterService)Activator.CreateInstance(type)!;
            instance.Register(services);
        }
    }
}