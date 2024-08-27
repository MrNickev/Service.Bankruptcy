using Application.Common.Models;
using Application.Fedresurs.Models;

namespace Application.Fedresurs.Abstractions;

public interface IApiProvider
{
    Task<Bankrupt<Person>?> FindBankrupt(Bankrupt bankrupt, CancellationToken cancellationToken = default);

    Task<PageData<Message>> FindMessages(Guid bankruptGuid, string[] courtDecisionTypes, int limit = 500,
        int offset = 0, CancellationToken cancellationToken = default);

    Task<PageData<Report>> FindReports(Guid bankruptGuid, string[] procedureTypes, int limit = 500, int offset = 0, CancellationToken cancellationToken = default);
}