using Application.Common.Models;

namespace Application.Common.Astractions;

/// <summary>
/// Сервис получения банкротов 
/// </summary>
public interface IBankruptcyCheckService
{
    /// <summary>
    /// Проверка, является ли человек банкротом
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<ClientBankruptcyCheckResult> Check(BankruptcyCheckRequest request);
    
    /// <summary>
    /// Проверка на банкротство для множества клиентов
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    // public Task<List<ClientBankruptcyCheckResult>> Check(List<BankruptcyCheckRequest> request);
}