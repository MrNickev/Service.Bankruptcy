using Application.Common.Models;

namespace Application.Fedresurs.Models;

/// <summary>
/// Результат проверки на банкрота
/// </summary>
public class BankruptCheckResult
{
    public BankruptCheckResult(BankruptStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Статус банкрота. Подробнее: <see cref="BankruptStatus"/>
    /// </summary>
    public BankruptStatus Status { get; set; }

}