using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models.Configuration;

///Ответ запроса авторизации 
public class AuthResponse
{
    /// <summary>
    /// JWT-тонен
    /// </summary>
    [JsonPropertyName("jwt")]
    public string JwtToken { get; set; }
}