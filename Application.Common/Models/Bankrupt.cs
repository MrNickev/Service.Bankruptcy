using System.Text.Json.Serialization;

namespace Application.Common.Models;

public class Bankrupt
{
    /// <summary>
    /// Тип лица
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BankruptType? Type { get; set; } = BankruptType.Person;
    
    /// <summary>
    /// Guid в системе Федресурс
    /// </summary>
    public Guid? Guid { get; set; }
    
    /// <summary>
    /// Имя лица
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// ОГРН (используется в случае если <see cref="BankruptType"/> == Company)
    /// </summary>
    public string? Ogrn { get; set; }
    
    /// <summary>
    /// ОГРНИП (используется в случае если <see cref="BankruptType"/> == Person)
    /// </summary>
    public string? Ogrnip { get; set; }
    
    /// <summary>
    /// ИНН
    /// </summary>
    public string? Inn { get; set; }
    
    /// <summary>
    /// СНИЛС (используется в случае если <see cref="BankruptType"/> == Person)
    /// </summary>
    public string? Snils { get; set; }
    
    /// <summary>
    /// Дата рождения
    /// </summary>
    public string? Birthdate { get; set; }
}