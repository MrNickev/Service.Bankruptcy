using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models.Configuration;

public class AuthResponse
{
    [JsonPropertyName("jwt")]
    public string JwtToken { get; set; }
}