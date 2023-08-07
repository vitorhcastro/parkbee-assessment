using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Authorization;
using Infrastructure.Gateways;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<ApiKeyAuthenticationHandler>();

        services.AddDbContext<ParkingDbContext>(options => options.UseInMemoryDatabase("parkbee"));

        services.AddScoped<IParkingDbContext>(provider => provider.GetRequiredService<ParkingDbContext>());

        services.AddTransient<IDateTime, DateTimeService>();

        services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme,
                null);

        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IDoorGateway, DoorGateway>();

        return services;
    }

    public static void AddSeedData(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ParkingDbContext>();

        dbContext.Garages.AddRange(Enumerable.Range(0, 10).Select(x => new Garage
        {
            Id = Guid.NewGuid(),
            Name = $"Garage {Guid.NewGuid()}",
            Doors = new List<Door>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = $"Door {Guid.NewGuid()}",
                    DoorType = DoorType.Entry
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = $"Door {Guid.NewGuid()}",
                    DoorType = DoorType.Exit
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = $"Door {Guid.NewGuid()}",
                    DoorType = DoorType.Pedestrian
                }
            }
        }));
        dbContext.Users.AddRange(Enumerable.Range(0, 20).Select(x => new User()
        {
            Id = Guid.NewGuid(),
            PartnerId = "partner-1"
        }));

        dbContext.SaveChanges();
    }
}
