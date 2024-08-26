using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureLayer.Configuration.Abstractions;

public interface IRegisterService
{
    void Register(IServiceCollection services);
}