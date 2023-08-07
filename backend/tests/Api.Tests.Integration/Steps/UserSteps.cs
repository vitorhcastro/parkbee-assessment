using System.Net;
using System.Net.Http.Json;
using Api.Tests.Integration.Drivers;
using Api.Tests.Integration.Fixtures;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace Api.Tests.Integration.Steps;

[Binding]
public class UserSteps : IClassFixture<IntegrationTestFixture>
{
    private readonly ScenarioContext scenarioContext;
    private readonly FeatureContext featureContext;
    private readonly IntegrationTestsDriver driver;

    public UserSteps(
        ScenarioContext scenarioContext,
        FeatureContext featureContext,
        IntegrationTestsDriver driver)
    {
        this.scenarioContext = scenarioContext;
        this.featureContext = featureContext;
        this.driver = driver;
    }


    [Given(@"the users exist in the system")]
    public async Task GivenTheUsersExistInTheSystem(Table table)
    {
        var parsedTable = table.CreateSet<UserTable>();
        var toCreate = new List<User>();
        foreach (var row in parsedTable)
        {
            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                PartnerId = ApplicationConstants.Authentication.DefaultPartnerId,
            };
            toCreate.Add(newUser);
            this.featureContext.Set(newUser, row.User);
        }

        this.driver.GetDatabaseContext().Users.AddRange(toCreate);
        await driver.GetDatabaseContext().SaveChangesAsync();
    }

    [Given(@"""(.*)"" has no running parking session")]
    public async Task GivenHasNoRunningParkingSession(string userKey)
    {
        var user = this.featureContext.Get<User>(userKey);
        var response = await this.driver.GetHttpClient().GetAsync($"/api/Users/{user.Id}/parking-sessions?status={ParkingSessionStatus.Running}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var parkingSessions = await response.Content.ReadFromJsonAsync<List<ParkingSession>>();
        parkingSessions.Should().BeEmpty();
        this.scenarioContext.Set(user, "current-user");
    }

    [Given(@"""(.*)"" has a running parking session in any garage")]
    public async Task GivenHasARunningParkingSessionInAnyGarage(string userKey)
    {
        var user = this.featureContext.Get<User>(userKey);
        var response = await this.driver.GetHttpClient().GetAsync($"/api/Users/{user.Id}/parking-sessions?status={ParkingSessionStatus.Running}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var parkingSessions = await response.Content.ReadFromJsonAsync<List<ParkingSession>>();
        parkingSessions.Should().NotBeEmpty();
        this.scenarioContext.Set(user, "current-user");
    }
}

public record UserTable(string User);
