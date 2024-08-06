using RulesEngineDemo.Application;
using RulesEngineDemo.Domain.Interfaces;
using RulesEngineDemo.Infrastructure.DI;

namespace RulesEngineDemo.Api.Extensions;

public static class AddServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddInfrastructure(config);
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();

        return services;
    }
}
