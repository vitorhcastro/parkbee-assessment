using Application.ParkingSessions.Commands.CreateParkingSession;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Moq;
using Presentation.Controllers;
using static TestHelpers.ParkingSessionBuilder;

namespace Presentation.Tests.Unit.Controllers.ParkingSessions;

public class CreateParkingSessionTests
{
    private static readonly ParkingSession TestParkingSession = AParkingSession().Build();

    private static readonly CreateParkingSessionRequest TestRequest =
        new(TestParkingSession.UserId, TestParkingSession.EntryDoor.GarageId, TestParkingSession.EntryDoorId);

    private readonly Mock<IMediator> mediatorMock;
    private readonly ParkingSessionsController parkingSessionsController;

    public CreateParkingSessionTests()
    {
        mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(new CreateParkingSessionCommand(TestRequest),
                default))
            .ReturnsAsync(new CreateParkingSessionResponse(TestParkingSession.Id));
        parkingSessionsController = new ParkingSessionsController(mediatorMock.Object);
    }

    [Fact]
    public async Task should_invoke_mediator_command()
    {
        // Act
        await parkingSessionsController.CreateParkingSession(TestRequest, CancellationToken.None);

        // Assert
        mediatorMock.Verify(
            m => m.Send(new CreateParkingSessionCommand(TestRequest),
                default),
            Times.Once);
    }

    [Fact]
    public async Task should_return_created_parking_session_id_from_command()
    {
        // Arrange
        var expected = new CreateParkingSessionResponse(TestParkingSession.Id);

        // Act
        var actual = await parkingSessionsController.CreateParkingSession(TestRequest, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
