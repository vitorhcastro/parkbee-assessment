using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Garages.Queries.GetGarageDoorHealth;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

using static TestHelpers.Builders.DoorBuilder;
using static TestHelpers.Builders.GarageBuilder;

namespace Application.Tests.Unit.Garages.Queries;

public class GetGarageDoorHealthQueryTests
{
    private static readonly Door TestDoor = ADoor().Build();
    private static readonly Garage TestGarage = AGarage().WithDoor(TestDoor).Build();

    private readonly Mock<IParkingDbContext> dbContextMock;
    private readonly Mock<IDoorGateway> gatewayMock;
    private readonly GetGarageDoorHealthQueryHandler queryHandler;

    public GetGarageDoorHealthQueryTests()
    {
        dbContextMock = new Mock<IParkingDbContext>();
        dbContextMock
            .Setup<DbSet<Garage>>(m => m.Garages)
            .ReturnsDbSet(new List<Garage> { TestGarage });
        dbContextMock
            .Setup<DbSet<Door>>(m => m.Doors)
            .ReturnsDbSet(new List<Door> { TestDoor });
        gatewayMock = new Mock<IDoorGateway>();
        queryHandler = new GetGarageDoorHealthQueryHandler(dbContextMock.Object, gatewayMock.Object);
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
            await queryHandler.Handle(
                new GetGarageDoorHealthQuery(TestGarage.Id, TestDoor.Id),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(Garage)}\" ({TestGarage.Id}) was not found.");
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
            await queryHandler.Handle(
                new GetGarageDoorHealthQuery(TestGarage.Id, TestDoor.Id),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(Door)}\" ({TestDoor.Id}) was not found.");
    }

    [Fact]
    public async Task should_invoke_gateway_to_get_health_status()
    {
        // Arrange
        this.gatewayMock
            .Setup(x => x.CheckHealth(TestDoor))
            .Returns(DoorHealth.Ok);

        // Act
        await queryHandler.Handle(
            new GetGarageDoorHealthQuery(TestGarage.Id, TestDoor.Id),
            CancellationToken.None);

        // Assert
        this.gatewayMock.Verify(m => m.CheckHealth(TestDoor), Times.Once);
    }

    [Fact]
    public async Task should_return_status_when_gateway_responds()
    {
        // Arrange
        this.gatewayMock
            .Setup(x => x.CheckHealth(TestDoor))
            .Returns(DoorHealth.Ok);
        var expected = new GetGarageDoorHealthResponse
        {
            Health = DoorHealth.Ok,
        };

        // Act
        var actual = await queryHandler.Handle(
            new GetGarageDoorHealthQuery(TestGarage.Id, TestDoor.Id),
            CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
