using Api.Tests.Integration.Fixtures;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Integration.Drivers;

public class IntegrationTestsDriver
{
    private readonly IntegrationTestFixture factory;
    private readonly HttpClient httpClient;

    private IServiceScope? currentScope;

    public IntegrationTestsDriver(IntegrationTestFixture factory)
    {
        this.factory = factory;
        httpClient = this.factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            BaseAddress = new Uri("https://localhost"),
        });
    }


    public IParkingDbContext GetDatabaseContext()
    {
        return GetCurrentScope().ServiceProvider.GetService<IParkingDbContext>()!;
    }

    public HttpClient GetHttpClient()
    {
        return httpClient;
    }

    public T GetService<T>()
    {
        return GetCurrentScope().ServiceProvider.GetService<T>()!;
    }

    private IServiceScope GetCurrentScope()
    {
        return currentScope ??= factory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
    }
}
