namespace Application.Fedresurs.Models;

public enum ProcedureType
{
    /// <summary>
    /// Финансовое оздоровление
    /// </summary>
    FinalRecovery,
    
    /// <summary>
    /// Внешнее управление
    /// </summary>
    ExternalManagment,
    
    /// <summary>
    /// Конкурсное производство
    /// </summary>
    Tender,
    
    /// <summary>
    /// Наблюдение
    /// </summary>
    Watching,
    
    /// <summary>
    /// Реализация имущества гражданина
    /// </summary>
    CitizenAssetsDisposal,
    
    /// <summary>
    /// Реструктуризация долгов гражданина
    /// </summary>
    CitizenDebtRestructuring
}