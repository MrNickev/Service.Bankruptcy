using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Application.Common.Models;

/// <summary>
/// Модель запроса
/// </summary>
public class BankruptcyCheckRequest : Bankrupt
{
    /// <summary>
    /// Идентификатор клиента. Нужен для идентификации клиента в ответе
    /// </summary>
    public int? ClientId { get; set; }
}