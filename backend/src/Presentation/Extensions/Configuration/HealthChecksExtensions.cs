using Infrastructure.Persistence;

namespace Presentation.Extensions.Configuration;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddHealthMonitoring(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<ParkingDbContext>();

        return services;
    }

    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");

        return app;
    }
}
