namespace Application.Fedresurs.Models;

public enum ReportType
{
    /// <summary>
    /// Финальный отчет
    /// </summary>
    Final,
    
    /// <summary>
    /// Аннулирование ранее опубликованного отчета
    /// </summary>
    Annulment,
    
    /// <summary>
    /// Отчет по существенным фактам
    /// </summary>
    SignificantEvent,
    
    /// <summary>
    /// Пириодический отчет
    /// </summary>
    Periodic,
    
    /// <summary>
    /// Финальный отчет v2
    /// </summary>
    Final2,
    
    /// <summary>
    /// Аннулирование ранее опубликованного отчета v2
    /// </summary>
    Annulment2
}