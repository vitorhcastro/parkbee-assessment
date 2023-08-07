using Application.ParkingSessions.Queries.GetParkingSessionById;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Moq;
using Presentation.Controllers;
using static TestHelpers.ParkingSessionBuilder;

namespace Presentation.Tests.Unit.Controllers.ParkingSessions;

public class GetParkingSessionByIdTests
{
    private static readonly ParkingSession TestParkingSession = AParkingSession().Build();
    private readonly Mock<IMediator> mediatorMock;
    private readonly ParkingSessionsController parkingSessionsController;

    public GetParkingSessionByIdTests()
    {
        mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(new GetParkingSessionByIdQuery(TestParkingSession.Id),
                default))
            .ReturnsAsync(TestParkingSession);
        parkingSessionsController = new ParkingSessionsController(mediatorMock.Object);
    }

    [Fact]
    public async Task should_invoke_mediator_query()
    {
        // Arrange

        // Act
        await parkingSessionsController.GetParkingSessionById(TestParkingSession.Id, CancellationToken.None);

        // Assert
        mediatorMock.Verify(
            m => m.Send(new GetParkingSessionByIdQuery(TestParkingSession.Id),
                default),
            Times.Once);
    }

    [Fact]
    public async Task should_return_health_status_from_query()
    {
        // Arrange
        mediatorMock
            .Setup(m => m.Send(new GetParkingSessionByIdQuery(TestParkingSession.Id),
                default))
            .ReturnsAsync(TestParkingSession);

        // Act
        var actual = await parkingSessionsController.GetParkingSessionById(TestParkingSession.Id, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(TestParkingSession);
    }
}
