using System.Text.Json.Serialization;

namespace Application.Fedresurs.Models;

/// <summary>
/// Класс отчета
/// </summary>
public class Report
{
    /// <summary>
    /// Guid отчета
    /// </summary>
    public Guid Guid { get; set; }
    
    /// <summary>
    /// Guid должника
    /// </summary>
    public Guid? BankruptGuid { get; set; }
    
    /// <summary>
    /// Номер отчета 
    /// </summary>
    public string Number { get; set; }
    
    /// <summary>
    /// Дата публикации отчета
    /// </summary>
    public DateTime DatePublish { get; set; }
    
    /// <summary>
    /// Тип отчета
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReportType Type { get; set; }
    
    /// <summary>
    /// Тип процедуры
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProcedureType? ProcedureType { get; set; }
    
    /// <summary>
    /// Причина блокировки. Возвращается только если отчет заблокирован
    /// </summary>
    public string? LockReason { get; set; }
    
    
    
}