using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.ParkingSessions.Commands.CreateParkingSession;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using static TestHelpers.DoorBuilder;
using static TestHelpers.GarageBuilder;
using static TestHelpers.ParkingSessionBuilder;

namespace Application.Tests.Unit.ParkingSessions.Commands;

public class CreateParkingSessionTests
{
    private static readonly ParkingSession TestParkingSession = AParkingSession().Build();

    private static readonly CreateParkingSessionRequest TestRequest =
        new(TestParkingSession.UserId, TestParkingSession.EntryDoor.GarageId, TestParkingSession.EntryDoorId);

    private readonly Mock<IParkingDbContext> dbContextMock;
    private readonly Mock<IDoorGateway> gatewayMock;
    private readonly CreateParkingSessionCommandHandler commandHandler;

    public CreateParkingSessionTests()
    {
        dbContextMock = new Mock<IParkingDbContext>();
        dbContextMock
            .Setup<DbSet<User>>(m => m.Users)
            .ReturnsDbSet(new List<User> { TestParkingSession.User });
        dbContextMock
            .Setup<DbSet<Garage>>(m => m.Garages)
            .ReturnsDbSet(new List<Garage> { TestParkingSession.EntryDoor.Garage });
        dbContextMock
            .Setup<DbSet<Door>>(m => m.Doors)
            .ReturnsDbSet(new List<Door> { TestParkingSession.EntryDoor });
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession>());
        dbContextMock
            .Setup(m => m.SaveChangesAsync(default))
            .ReturnsAsync(1);

        gatewayMock = new Mock<IDoorGateway>();
        gatewayMock
            .Setup(x => x.CheckHealth(TestParkingSession.EntryDoor))
            .ReturnsAsync(DoorHealth.Ok);

        commandHandler = new CreateParkingSessionCommandHandler(dbContextMock.Object, gatewayMock.Object);
    }

    [Fact]
    public async Task should_throw_not_found_exception_when_user_does_not_exist()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<User>>(m => m.Users)
            .ReturnsDbSet(new List<User>());

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(User)}\" ({TestRequest.UserId}) was not found.");
    }

    [Fact]
    public async Task should_throw_not_found_exception_when_garage_does_not_exist()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<Garage>>(m => m.Garages)
            .ReturnsDbSet(new List<Garage>());

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(Garage)}\" ({TestRequest.GarageId}) was not found.");
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
            await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(Door)}\" ({TestRequest.DoorId}) was not found.");
    }

    [Fact]
    public async Task should_throw_not_found_exception_when_door_does_not_belong_to_garage()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<Door>>(m => m.Doors)
            .ReturnsDbSet(new List<Door> { ADoor().WithId(TestRequest.DoorId).Build() });

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(Door)}\" ({TestRequest.DoorId}) was not found.");
    }

    [Fact]
    public async Task should_return_not_return_empty_id_after_creating_parking_session()
    {
        // Act
        var act = await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest),
            CancellationToken.None);

        // Assert
        act.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task should_invoke_door_gateway_to_open_door()
    {
        // Arrange
        this.gatewayMock
            .Setup(x => x.OpenDoor(TestParkingSession.EntryDoor))
            .Returns(Task.CompletedTask);

        // Act
        await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest),
            CancellationToken.None);

        // Assert
        this.gatewayMock.Verify(m => m.OpenDoor(TestParkingSession.EntryDoor), Times.Once);
    }

    [Fact]
    public async Task should_throw_validation_exception_if_user_already_has_running_session()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession> { AParkingSession().WithUser(TestParkingSession.User).Build() });

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest),
                CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task should_throw_validation_exception_if_garage_has_no_available_spots()
    {
        // Arrange
        var door = ADoor().Build();
        var garage = AGarage().WithTotalSpots(1).WithDoor(door).Build();
        door.GarageId = garage.Id;
        var parkingSession = AParkingSession().WithEntryDoor(door).Build();
        dbContextMock
            .Setup<DbSet<Garage>>(m => m.Garages)
            .ReturnsDbSet(new List<Garage> { garage });
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession> { parkingSession });
        dbContextMock
            .Setup<DbSet<User>>(m => m.Users)
            .ReturnsDbSet(new List<User> { parkingSession.User });
        dbContextMock
            .Setup<DbSet<Door>>(m => m.Doors)
            .ReturnsDbSet(new List<Door> { door });

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(
                new CreateParkingSessionCommand(
                    new CreateParkingSessionRequest(
                        parkingSession.UserId,
                        garage.Id,
                        door.Id)),
                CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(DoorHealth.Unreachable)]
    [InlineData(DoorHealth.Unknown)]
    public async Task should_throw_validation_exception_if_door_is_not_ok(DoorHealth doorHealth)
    {
        // Arrange
        this.gatewayMock
            .Setup(m => m.CheckHealth(TestParkingSession.EntryDoor))
            .ReturnsAsync(doorHealth);

        // Act
        Func<Task> act = async () =>
            await commandHandler.Handle(new CreateParkingSessionCommand(TestRequest), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
