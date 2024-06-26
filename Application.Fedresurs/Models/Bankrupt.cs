using System.Text.Json.Serialization;
using Application.Common.Models;

namespace Application.Fedresurs.Models;

/// <summary>
/// Модель банкрота
/// </summary>
/// <typeparam name="T"></typeparam>
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
    
    /// <summary>
    /// Данные о банкроте
    /// </summary>
    public T Data { get; set; }
    
    /// <summary>
    /// Сообщения по банкроту
    /// </summary>
    [JsonIgnore]
    public List<Message> Messages { get; set; }

    /// <summary>
    /// Отчеты по банкроту
    /// </summary>
    [JsonIgnore]
    public List<Report> Reports { get; set; }
}