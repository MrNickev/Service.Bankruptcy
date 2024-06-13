namespace Application.Common.Models;

public enum BankruptStatus
{
    /// <summary>
    /// Не банкрот
    /// </summary>
    NotBankrupt,
    
    /// <summary>
    /// Потенциальный банкрот - подано заявление на банкротство
    /// </summary>
    PossibleBankruptcy,
    
    /// <summary>
    /// Процедурный банкрот - идет делопроизводство
    /// </summary>
    ProceduralBankruptcy,
    
    /// <summary>
    /// Конченный банкрот - делопроизводство окончено, признан банкротом 
    /// </summary>
    FinishedBankruptcy
}