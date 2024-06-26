using Newtonsoft.Json;

namespace Application.Fedresurs.Models.Configuration;

/// <summary>
/// Конфигурация для взаимодействия с API Федресурс
/// </summary>
public class FedresursConfiguration
{
    /// <summary>
    /// Логин для API Федресурса
    /// </summary>
    [JsonProperty(nameof(Login))]
    public string Login { get; set; }
    
    /// <summary>
    /// Пароль для API Федресурса
    /// </summary>
    [JsonProperty(nameof(Password))]
    public string Password { get; set; }
    
    /// <summary>
    /// Адрес Федресурс API
    /// </summary>
    [JsonProperty(nameof(Host))]
    public string Host { get; set; }
}