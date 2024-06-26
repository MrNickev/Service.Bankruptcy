using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models;

public class Message
{
    /// <summary>
    /// Guid сообщения
    /// </summary>
    [JsonPropertyName("guid")]
    public Guid Guid { get; set; }
    
    /// <summary>
    /// Guid должника
    /// </summary>
    [JsonPropertyName("bankruptGuid")]
    public Guid BankruptGuid { get; set; }
    
    /// <summary>
    /// Guid сообщения об аннулировании
    /// </summary>
    [JsonPropertyName("annulmentMessageGuid")]
    public Guid? AnnulmentMessageGuid { get; set; }
    
    /// <summary>
    /// Номер сообщения
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; set; }
    
    /// <summary>
    /// Дата публикации сообщения
    /// </summary>
    [JsonPropertyName("datePublish")]
    public DateTime DatePublish { get; set; }
    
    /// <summary>
    /// Содержание сообщения
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    /// <summary>
    /// Тип сообщения
    /// </summary>
    [JsonPropertyName("type")]
    // [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public string? Type { get; set; }
    
    /// <summary>
    /// Причина блокировки. Возвращается только если сообщение заблокировано
    /// </summary>
    [JsonPropertyName("lockReason")]
    public string? LockReason { get; set; }  
    
    /// <summary>
    /// Признак публикации с нарушением сроков.
    /// true - в результат попадет сообщение с нарушением сроков
    /// false или null - в результат НЕ попадет сообщение с нарушением сроков
    /// </summary>
    [JsonPropertyName("hasViolation")]
    public bool? HasViolation { get; set; }
    
}