using Application.Common.Models;
using Application.Fedresurs.Models;

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
    public Task<BankruptcyCheckResult> Check(BankruptcyCheckRequest request);
}