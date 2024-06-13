using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models;

public class Bankrupt<T>
{
    /// <summary>
    /// GUID должника
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Тип должника
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BankruptType Type { get; set; }
    
    public T Data { get; set; }

}