using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RulesEngineDemo.Infrastructure.Data;

namespace RulesEngineDemo.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<LeaveDbContext>(options => options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        return services;
    }
}
