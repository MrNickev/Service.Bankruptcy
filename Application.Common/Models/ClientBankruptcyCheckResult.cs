namespace Application.Common.Models;

/// <summary>
/// Модель проверки на банкроство с информацией о клиенте з запроса
/// </summary>
public class ClientBankruptcyCheckResult
{
    /// <summary>
    /// Поступивший запрос
    /// </summary>
    public BankruptcyCheckRequest Request { get; set; }

    /// <summary>
    /// Результаты проверки
    /// </summary>
    public List<BankruptcyCheckResult> Results { get; set; }

    public ClientBankruptcyCheckResult(BankruptcyCheckRequest request, List<BankruptcyCheckResult> results)
    {
        Request = request;
        Results = results;
    }

    public ClientBankruptcyCheckResult(BankruptcyCheckRequest request, BankruptStatus bankruptStatus, string serviceName)
    {
        Request = request;
        Results = new List<BankruptcyCheckResult>() { new BankruptcyCheckResult(bankruptStatus, serviceName) };
    }
}