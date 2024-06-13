using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models;

/// <summary>
/// Модель для авторизации в API Федресурса
/// </summary>
public class AuthRequest
{
    [JsonPropertyName("login")]
    public string Login;
    
    [JsonPropertyName("password")]
    public string Password;

    public AuthRequest(string login, string password)
    {
        Login = login;
        Password = password;
    }
}