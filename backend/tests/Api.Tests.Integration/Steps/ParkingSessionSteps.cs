using System.Net;
using System.Net.Http.Json;
using Api.Tests.Integration.Drivers;
using Application.ParkingSessions.Commands.CreateParkingSession;
using Domain.Entities;
using FluentAssertions;

namespace Api.Tests.Integration.Steps;

[Binding]
public class ParkingSessionSteps
{
    private readonly ScenarioContext scenarioContext;
    private readonly FeatureContext featureContext;
    private readonly IntegrationTestsDriver driver;

    public ParkingSessionSteps(
        ScenarioContext scenarioContext,
        FeatureContext featureContext,
        IntegrationTestsDriver driver)
    {
        this.scenarioContext = scenarioContext;
        this.featureContext = featureContext;
        this.driver = driver;
    }

    [When(@"Start parking session API endpoint is called")]
    public async Task WhenStartParkingSessionApiEndpointIsCalled()
    {
        var currentUser = this.scenarioContext.Get<User>("current-user");
        var currentGarage = this.scenarioContext.Get<Garage>("current-garage");
        var currentDoor = this.scenarioContext.Get<Door>("current-door");
        var parkingSession = new CreateParkingSessionRequest(currentUser.Id, currentGarage.Id, currentDoor.Id);
        var response = await this.driver.GetHttpClient().PostAsJsonAsync("/api/parking-sessions", parkingSession);
        this.scenarioContext.Set(response, "create-parking-session-response");
    }

    [Then(@"Endpoint should return a successful response with parking session id")]
    public async Task ThenEndpointShouldReturnASuccessfulResponseWithParkingSessionId()
    {
        var response = this.scenarioContext.Get<HttpResponseMessage>("create-parking-session-response");
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var parkingSession = await response.Content.ReadFromJsonAsync<CreateParkingSessionResponse>();
        parkingSession.Should().NotBeNull();
        parkingSession.Id.Should().NotBeEmpty();
    }

    [Then(@"Endpoint should return an error code")]
    public void ThenEndpointShouldReturnAnErrorCode()
    {
        var response = this.scenarioContext.Get<HttpResponseMessage>("create-parking-session-response");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Then(@"New parking session should not be created")]
    public async Task ThenNewParkingSessionShouldNotBeCreated()
    {
        var response = this.scenarioContext.Get<HttpResponseMessage>("create-parking-session-response");
        var parkingSession = await response.Content.ReadFromJsonAsync<CreateParkingSessionResponse>();
        parkingSession.Should().BeNull();
    }

    [Given(@"parking session exists for ""(.*)"" in ""(.*)"" and is running")]
    public async Task GivenParkingSessionExistsForInAndIsRunning(string userKey, string garageKey)
    {
        var currentUser = this.featureContext.Get<User>(userKey);
        var currentGarage = this.featureContext.Get<Garage>(garageKey);
        var door = currentGarage.Doors.First(x => x.DoorType == DoorType.Entry);
        var parkingSession = new CreateParkingSessionRequest(currentUser.Id, currentGarage.Id, door.Id);
        var response = await this.driver.GetHttpClient().PostAsJsonAsync("/api/parking-sessions", parkingSession);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdParkingSession = await response.Content.ReadFromJsonAsync<CreateParkingSessionResponse>();
        this.scenarioContext.Set(createdParkingSession, "create-parking-session-response");
    }

    [When(@"Stop parking session API endpoint is called")]
    public async Task WhenStopParkingSessionApiEndpointIsCalled()
    {
        var createdParkingSession = this.scenarioContext.Get<CreateParkingSessionResponse>("create-parking-session-response");
        var garage = this.scenarioContext.Get<Garage>("current-garage");
        var exitDoor = garage.Doors.First(x => x.DoorType == DoorType.Exit);
        var stopRequest = new UpdateParkingSessionStatusRequest
        {
            Status = ParkingSessionStatus.Stopped,
            ExitDoorId = exitDoor.Id,
        };
        var response = await this.driver.GetHttpClient().PutAsJsonAsync($"/api/parking-sessions/{createdParkingSession.Id}/status", stopRequest);
        this.scenarioContext.Set(response, "stop-parking-session-response");
        this.scenarioContext.Set(exitDoor, "current-door");
    }

    [Then(@"Endpoint returns a success code")]
    public void ThenEndpointReturnsASuccessCode()
    {
        var response = this.scenarioContext.Get<HttpResponseMessage>("stop-parking-session-response");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Then(@"Parking session should be stopped")]
    public async Task ThenParkingSessionShouldBeStopped()
    {
        var createdParkingSession = this.scenarioContext.Get<CreateParkingSessionResponse>("create-parking-session-response");
        var response = await this.driver.GetHttpClient().GetAsync($"/api/parking-sessions/{createdParkingSession.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var parkingSession = await response.Content.ReadFromJsonAsync<GetParkingSessionByIdResponse>();
        parkingSession.Status.Should().Be(ParkingSessionStatus.Stopped);
        this.scenarioContext.Set(response, "stop-parking-session-response");
    }
}
