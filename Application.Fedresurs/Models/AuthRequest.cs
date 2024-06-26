using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models;

/// <summary>
/// Модель для авторизации в API Федресурса
/// </summary>
public class AuthRequest
{
    /// <summary>
    /// Логин
    /// </summary>
    [JsonPropertyName("login")]
    public string Login;
    
    /// <summary>
    /// Пароль
    /// </summary>
    [JsonPropertyName("password")]
    public string Password;

    /// <summary>
    /// Конуструкутор конфигурации AythRequest
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    public AuthRequest(string login, string password)
    {
        Login = login;
        Password = password;
    }
}