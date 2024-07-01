using System.Text.Json.Serialization;
using Application.Common.Models;

namespace Application.Fedresurs.Models;

/// <summary>
/// Результат проверки на банкрота
/// </summary>
public class BankruptcyCheckResult
{
    public BankruptcyCheckResult(BankruptStatus status, string serviceName)
    {
        Status = status;
        ServiceName = serviceName;
    }

    /// <summary>
    /// Статус банкрота. Подробнее: <see cref="BankruptStatus"/>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BankruptStatus Status { get; set; }
    
    /// <summary>
    /// Имя сервиса, в котором происходила проверка
    /// </summary>
    public string ServiceName { get; set; }

}