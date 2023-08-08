using System.Net;
using System.Net.Http.Json;
using Api.Tests.Integration.Drivers;
using Api.Tests.Integration.Fixtures;
using Application.Garages.Queries.GetGarageById;
using Application.Garages.Queries.GetGarageDoorHealth;
using Application.Garages.Queries.GetGarageDoorStatus;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace Api.Tests.Integration.Steps;

[Binding]
public class GarageSteps : IClassFixture<IntegrationTestFixture>
{
    private readonly ScenarioContext scenarioContext;
    private readonly FeatureContext featureContext;
    private readonly IntegrationTestsDriver driver;

    public GarageSteps(
        ScenarioContext scenarioContext,
        FeatureContext featureContext,
        IntegrationTestsDriver driver)
    {
        this.scenarioContext = scenarioContext;
        this.featureContext = featureContext;
        this.driver = driver;
    }

    [Given(@"the garages exist in the system")]
    public async Task GivenTheGaragesExistInTheSystem(Table table)
    {
        var parsedTable = table.CreateSet<GarageTable>();
        var toCreate = new List<Garage>();
        foreach (var row in parsedTable)
        {
            var newGarage = new Garage()
            {
                Id = Guid.NewGuid(),
                Name = row.Garage,
                TotalSpots = row.TotalSpots,
                Doors = new List<Door>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "Entry",
                        DoorType = DoorType.Entry,
                        IpAddress = row.DoorHealth == DoorHealth.Ok ? ApplicationConstants.ParkbeeDotComIpAddress : string.Empty,
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "Exit",
                        DoorType = DoorType.Exit,
                        IpAddress = ApplicationConstants.ParkbeeDotComIpAddress,
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Description = "Pedestrian",
                        DoorType = DoorType.Pedestrian,
                        IpAddress = ApplicationConstants.ParkbeeDotComIpAddress,
                    },
                }
            };
            toCreate.Add(newGarage);
            this.featureContext.Set(newGarage, row.Garage);
        }

        this.driver.GetDatabaseContext().Garages.AddRange(toCreate);
        await this.driver.GetDatabaseContext().SaveChangesAsync();
    }

    [Given(@"""(.*)"" has available spots")]
    public async Task GivenHasAvailableSpots(string garageKey)
    {
        var garage = this.featureContext.Get<Garage>(garageKey);
        var response = await this.driver.GetHttpClient().GetAsync($"/Garages/{garage.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var garageByIdDto = await response.Content.ReadFromJsonAsync<GetGarageByIdResponse>();
        garageByIdDto.AvailableSpots.Should().BeGreaterThan(0);
        this.scenarioContext.Set(garage, "current-garage");
    }

    [Given(@"location hardware is reachable")]
    public async Task GivenLocationHardwareIsReachable()
    {
        var garage = this.scenarioContext.Get<Garage>("current-garage");
        var door = garage.Doors.First(x => x.DoorType == DoorType.Entry);
        var response = await this.driver.GetHttpClient().GetAsync($"/Garages/{garage.Id}/doors/{door.Id}/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var health = await response.Content.ReadFromJsonAsync<GetGarageDoorHealthResponse>();
        health.Health.Should().Be(DoorHealth.Ok);
        this.scenarioContext.Set(door, "current-door");
    }

    [Then(@"(.*) door should open")]
    public async Task ThenEntryDoorShouldOpen(string doorType)
    {
        var doorTypeParsed = Enum.Parse<DoorType>(doorType);
        var currentDoor = this.scenarioContext.Get<Door>("current-door");
        currentDoor.DoorType.Should().Be(doorTypeParsed);
        var response = await this.driver.GetHttpClient().GetAsync($"/Garages/{currentDoor.GarageId}/doors/{currentDoor.Id}/status");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var health = await response.Content.ReadFromJsonAsync<GetGarageDoorStatusResponse>();
        health.Status.Should().Be(DoorStatus.Open);
    }

    [Given(@"""(.*)"" has no parking spots available")]
    public async Task GivenHasNoParkingSpotsAvailable(string garageKey)
    {
        var garage = this.featureContext.Get<Garage>(garageKey);
        var response = await this.driver.GetHttpClient().GetAsync($"/Garages/{garage.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var garageByIdDto = await response.Content.ReadFromJsonAsync<GetGarageByIdResponse>();
        garageByIdDto.AvailableSpots.Should().Be(0);
        this.scenarioContext.Set(garage, "current-garage");
        this.scenarioContext.Set(garage.Doors.First(x => x.DoorType == DoorType.Entry), "current-door");
    }

    [Given(@"""(.*)"" (.*) door hardware is not reachable")]
    public async Task GivenEntryDoorHardwareIsNotReachable(string garageKey, string doorType)
    {
        var garage = this.featureContext.Get<Garage>(garageKey);
        var doorTypeParsed = Enum.Parse<DoorType>(doorType);
        var door = garage.Doors.First(x => x.DoorType == doorTypeParsed);
        var response = await this.driver.GetHttpClient().GetAsync($"/Garages/{garage.Id}/doors/{door.Id}/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var health = await response.Content.ReadFromJsonAsync<GetGarageDoorHealthResponse>();
        health.Health.Should().Be(DoorHealth.Unreachable);
        this.scenarioContext.Set(door, "current-door");
    }
}

public record GarageTable(string Garage, int TotalSpots, DoorHealth DoorHealth);
