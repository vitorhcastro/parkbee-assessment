using Application.Garages.Queries.GetGarageDoorHealth;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Moq;
using Presentation.Controllers;
using static TestHelpers.DoorBuilder;
using static TestHelpers.GarageBuilder;

namespace Presentation.Tests.Unit.Controllers.Garages;

public class GetGarageDoorHealthTests
{
    private static readonly Door TestDoor = ADoor().Build();
    private static readonly Garage TestGarage = AGarage().WithDoor(TestDoor).Build();
    private readonly Mock<IMediator> mediatorMock;
    private readonly GaragesController garagesController;

    public GetGarageDoorHealthTests()
    {
        mediatorMock = new Mock<IMediator>();
        garagesController = new GaragesController(mediatorMock.Object);
    }

    [Fact]
    public async Task should_invoke_mediator_query()
    {
        // Arrange
        mediatorMock
            .Setup(m => m.Send(new GetGarageDoorHealthQuery(TestGarage.Id, TestDoor.Id, CancellationToken.None),
                default))
            .ReturnsAsync(new GetGarageDoorHealthResponse());

        // Act
        await garagesController.GetGarageDoorHealth(TestGarage.Id, TestDoor.Id, CancellationToken.None);

        // Assert
        mediatorMock.Verify(
            m => m.Send(new GetGarageDoorHealthQuery(TestGarage.Id, TestDoor.Id, CancellationToken.None),
                default),
            Times.Once);
    }

    [Fact]
    public async Task should_return_health_status_from_query()
    {
        // Arrange
        var expected = new GetGarageDoorHealthResponse
        {
            Health = DoorHealthStatus.Ok,
        };
        mediatorMock
            .Setup(m => m.Send(new GetGarageDoorHealthQuery(TestGarage.Id, TestDoor.Id, CancellationToken.None),
                default))
            .ReturnsAsync(expected);

        // Act
        var actual = await garagesController.GetGarageDoorHealth(TestGarage.Id, TestDoor.Id, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}