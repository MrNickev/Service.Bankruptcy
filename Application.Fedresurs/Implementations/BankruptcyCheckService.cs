using Application.Common.Astractions;
using Application.Common.Models;
using Application.Fedresurs.Abstractions;
using Application.Fedresurs.Models;
using Application.Fedresurs.Models.Configuration;

namespace Application.Fedresurs.Implementations;

/// <inheritdoc/>
public class BankruptcyCheckService(IAuthService authService, FedresursConfiguration configuration) : IBankruptcyCheckService
{
    private readonly HttpClient _httpClient = new HttpClient();

    /// <inheritdoc /> 
    public async Task<BankruptCheckResult> Check(BankruptcyCheckRequest request)
    {
        var token = await authService.GetToken();
        var bankrupt = FindBunkrupt(request);

        if (bankrupt.Data.Count == 0)
        {
            return new BankruptCheckResult(BankruptStatus.NotBankrupt);
        }

        if (bankrupt.Data.Count == 1)
        {
            var messages = FindMessages(bankrupt.Data[0].Guid);
        }
        
        
        return new BankruptCheckResult(BankruptStatus.NotBankrupt);
    }

    public PageData<Bankrupt<Person>> FindBunkrupt(BankruptcyCheckRequest request)
    {
        
        return new PageData<Bankrupt<Person>>();
    }

    public PageData<Message> FindMessages(Guid bankruptGuid)
    {
        return new PageData<Message>();
    }
    
}