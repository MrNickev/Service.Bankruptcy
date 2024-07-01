using System.Text.Json.Serialization;

namespace Application.Common.Models;

/// <summary>
/// Модель запроса
/// </summary>
public class BankruptcyCheckRequest
{
    /// <summary>
    /// Тип лица
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BankruptType? Type { get; set; }
    
    /// <summary>
    /// Guid в системе Фересурс
    /// </summary>
    public Guid? Guid { get; set; }
    
    /// <summary>
    /// Имя лица
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// ОГРН (используется в случае если <see cref="BunkruptType"/> == Company)
    /// </summary>
    public string? Ogrn { get; set; }
    
    /// <summary>
    /// ОГРНИП (используется в случае если <see cref="BunkruptType"/> == Person)
    /// </summary>
    public string? Ogrnip { get; set; }
    
    /// <summary>
    /// ИНН
    /// </summary>
    public string? Inn { get; set; }
    
    /// <summary>
    /// СНИЛС (используется в случае если <see cref="BunkruptType"/> == Person)
    /// </summary>
    public string? Snils { get; set; }
    
    /// <summary>
    /// Дата рождения
    /// </summary>
    public string? Birthdate { get; set; }
    
    /// <summary>
    /// Количество возвращаемых записей. Максимум 500
    /// </summary>
    public int Limit { get; set; }
    
    /// <summary>
    /// Сдвиг. Количество записей, которые будут пропущены, начиная с первой
    /// </summary>
    public int Offset { get; set; }
}