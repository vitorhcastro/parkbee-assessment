using Application.ParkingSessions.Commands.UpdateParkingSessionStatus;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using static TestHelpers.Builders.DoorBuilder;
using static TestHelpers.Builders.ParkingSessionBuilder;

namespace Presentation.Tests.Unit.Controllers.ParkingSessions;

public class UpdateParkingSessionStatusTests
{
    private static readonly ParkingSession TestParkingSession = AParkingSession().Build();
    private static readonly Door TestDoor = ADoor().Build();

    private static readonly UpdateParkingSessionStatusRequest TestRequest =
        new(TestDoor.Id, ParkingSessionStatus.Stopped);

    private readonly Mock<IMediator> mediatorMock;
    private readonly ParkingSessionsController parkingSessionsController;

    public UpdateParkingSessionStatusTests()
    {
        mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(new UpdateParkingSessionStatusCommand(TestParkingSession.Id, TestRequest),
                default))
            .ReturnsAsync(new MediatR.Unit());
        parkingSessionsController = new ParkingSessionsController(mediatorMock.Object);
    }

    [Fact]
    public async Task should_invoke_mediator_command()
    {
        // Act
        await parkingSessionsController.UpdateParkingSessionStatus(TestParkingSession.Id, TestRequest, CancellationToken.None);

        // Assert
        mediatorMock.Verify(
            m => m.Send(new UpdateParkingSessionStatusCommand(TestParkingSession.Id, TestRequest),
                default),
            Times.Once);
    }

    [Fact]
    public async Task should_return_no_content_after_command()
    {
        // Act
        var actual = await parkingSessionsController.UpdateParkingSessionStatus(TestParkingSession.Id, TestRequest, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(new NoContentResult());
    }
}
