using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Common.Astractions;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Models;
using Application.Fedresurs.Models.Configuration;
using System.Threading.RateLimiting;

namespace Application.Fedresurs.Implementations;

/// <summary>
/// Сервис проверки на банкротство через API Федресурса
/// </summary>
public class FedresursBankruptcyCheckService(IApiProvider apiProvider) : IBankruptcyCheckService
{


    /// <inheritdoc /> 
    public async Task<ClientBankruptcyCheckResult> Check(BankruptcyCheckRequest request)
    {
        //сначала делаем поиск по всем данным которые есть, если ничего не нашли, то пробуем искать отдельно по ИНН и по СНИЛС
        //имена в нашей базе и базе федресурса могут различаться, поэтому лучше искать по ИНН или СНИЛС - что-то из этого еть всегда
        var bankrupt = await apiProvider.FindBankrupt(request) ?? 
                       await apiProvider.FindBankrupt(new BankruptcyCheckRequest() {Type = request.Type, Inn = request.Inn}) ?? 
                       await apiProvider.FindBankrupt(new BankruptcyCheckRequest() {Type = request.Type, Snils = request.Snils});

        if (bankrupt is null)
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.NotBankrupt, GetType().Name);
        }
            
        var finishedBankruptcyMessages = await apiProvider.FindMessages(bankrupt.Guid, new []
        {
            "LegalCaseEnd",
            "PropertySaleComplete"
        });
        
        
        //проверяем есть ли сообщения об окончании банкротства и актуальны ли они
        if (finishedBankruptcyMessages.Total > 0 && finishedBankruptcyMessages.Data.Any(m => m.DatePublish > DateTime.Today.AddYears(-3)))
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.FinishedBankruptcy, GetType().Name);
        }

        
        var refusalBankruptcyMessages = await apiProvider.FindMessages(bankrupt.Guid, new[]
        {
            "BankruptcyRefusal",
            "ObligationsDischargeRefusal"
        });
        
        if (refusalBankruptcyMessages.Total > 0 && refusalBankruptcyMessages.Data.Any(m => m.DatePublish > DateTime.Today.AddYears(-3)))
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.RefusalBankruptcy, GetType().Name);
        }
        

        var proceduralBankruptcyMessages = await apiProvider.FindMessages(bankrupt.Guid, new[]
        {
            "Receivership",
            "DebtRestructuring",
            "PropertySale",
        });

        var proceduralBankruptcyReports = await apiProvider.FindReports(bankrupt.Guid, new[]
        {
            "Tender",
            "Watching",
            "CitizenAssetsDisposal",
            "CitizenDebtRestructuring"
        });

        if (proceduralBankruptcyMessages.Total > 0 || proceduralBankruptcyReports.Total > 0)
        {
            return new ClientBankruptcyCheckResult(request, BankruptStatus.ProceduralBankruptcy, GetType().Name);
        }

        return new ClientBankruptcyCheckResult(request, BankruptStatus.NotBankrupt, GetType().Name);
    }
    
}