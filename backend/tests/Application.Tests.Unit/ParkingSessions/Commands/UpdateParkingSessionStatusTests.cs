using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.ParkingSessions.Commands.UpdateParkingSessionStatus;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using static TestHelpers.DoorBuilder;
using static TestHelpers.ParkingSessionBuilder;

namespace Application.Tests.Unit.ParkingSessions.Commands;

public class UpdateParkingSessionStatusTests
{
    private static readonly ParkingSession TestParkingSession = AParkingSession().Build();
    private static readonly Door TestDoor = ADoor().Build();

    private static readonly UpdateParkingSessionStatusRequest TestRequest =
        new(TestDoor.Id, ParkingSessionStatus.Stopped);

    private readonly Mock<IParkingDbContext> dbContextMock;
    private readonly Mock<IDoorGateway> gatewayMock;
    private readonly UpdateParkingSessionStatusCommandHandler commandHandler;

    public UpdateParkingSessionStatusTests()
    {
        dbContextMock = new Mock<IParkingDbContext>();
        dbContextMock
            .Setup<DbSet<Door>>(m => m.Doors)
            .ReturnsDbSet(new List<Door> { TestParkingSession.EntryDoor, TestDoor });
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession> { TestParkingSession });
        dbContextMock
            .Setup(m => m.SaveChangesAsync(default))
            .ReturnsAsync(1);

        gatewayMock = new Mock<IDoorGateway>();

        commandHandler = new UpdateParkingSessionStatusCommandHandler(dbContextMock.Object, gatewayMock.Object);
    }

    [Fact]
    public async Task should_throw_not_found_exception_when_parking_session_does_not_exist()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession>());

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new UpdateParkingSessionStatusCommand(TestParkingSession.Id, TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(ParkingSession)}\" ({TestParkingSession.Id}) was not found.");
    }

    [Fact]
    public async Task should_throw_not_found_exception_when_door_does_not_exist()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<Door>>(m => m.Doors)
            .ReturnsDbSet(new List<Door>());

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new UpdateParkingSessionStatusCommand(TestParkingSession.Id, TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(Door)}\" ({TestRequest.DoorId}) was not found.");
    }

    [Fact]
    public async Task should_throw_validation_exception_when_parking_session_is_not_running()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession>
                { AParkingSession().WithId(TestParkingSession.Id).WithStatus(ParkingSessionStatus.Stopped).Build() });

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new UpdateParkingSessionStatusCommand(TestParkingSession.Id, TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task should_update_and_save_parking_sessions()
    {
        // Act
        await commandHandler.Handle(new UpdateParkingSessionStatusCommand(TestParkingSession.Id, TestRequest),
                CancellationToken.None);

        // Assert
        this.dbContextMock.Verify(m => m.SetModified(It.IsAny<ParkingSession>()), Times.Once);
    }

    [Fact]
    public async Task should_invoke_gateway_to_open_door()
    {
        // Arrange
        var parkingSessions = AParkingSession().Build();
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession> { parkingSessions });
        this.gatewayMock.Setup(m => m.OpenDoor(TestDoor)).Returns(Task.CompletedTask);

        // Act
        await commandHandler.Handle(new UpdateParkingSessionStatusCommand(parkingSessions.Id, TestRequest),
                CancellationToken.None);

        // Assert
        this.gatewayMock.Verify(m => m.OpenDoor(TestDoor), Times.Once);
    }
}
