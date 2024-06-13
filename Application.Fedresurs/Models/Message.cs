using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models;

public class Message
{
    /// <summary>
    /// Guid сообщения
    /// </summary>
    public Guid Guid { get; set; }
    
    /// <summary>
    /// Guid должника
    /// </summary>
    public Guid BankruptGuid { get; set; }
    
    /// <summary>
    /// Guid сообщения об аннулировании
    /// </summary>
    public Guid? AnnulmentMessageGuid { get; set; }
    
    /// <summary>
    /// Номер сообщения
    /// </summary>
    public int Number { get; set; }
    
    /// <summary>
    /// Дата публикации сообщения
    /// </summary>
    public DateTime DatePublish { get; set; }
    
    /// <summary>
    /// Содержание сообщения
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// Тип сообщения
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageType Type { get; set; }
    
    /// <summary>
    /// Причина блокировки. Возвращается только если сообщение заблокировано
    /// </summary>
    public string? LockReason { get; set; }  
    
    /// <summary>
    /// Признак публикации с нарушением сроков.
    /// true - в результат попадет сообщение с нарушением сроков
    /// false или null - в результат НЕ попадет сообщение с нарушением сроков
    /// </summary>
    public bool? HasViolation { get; set; }
    
}