using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Garages.Queries.GetGarageDoorStatus;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using TestHelpers.Builders;

namespace Application.Tests.Unit.Garages.Queries;

using static DoorBuilder;
using static GarageBuilder;

public class GetGarageDoorStatusQueryTests
{
    private static readonly Door TestDoor = ADoor().Build();
    private static readonly Garage TestGarage = AGarage().WithDoor(TestDoor).Build();

    private readonly Mock<IParkingDbContext> dbContextMock;
    private readonly Mock<IDoorGateway> gatewayMock;
    private readonly GetGarageDoorStatusQueryHandler queryHandler;

    public GetGarageDoorStatusQueryTests()
    {
        dbContextMock = new Mock<IParkingDbContext>();
        dbContextMock
            .Setup<DbSet<Garage>>(m => m.Garages)
            .ReturnsDbSet(new List<Garage> { TestGarage });
        dbContextMock
            .Setup<DbSet<Door>>(m => m.Doors)
            .ReturnsDbSet(new List<Door> { TestDoor });
        gatewayMock = new Mock<IDoorGateway>();
        queryHandler = new GetGarageDoorStatusQueryHandler(dbContextMock.Object, gatewayMock.Object);
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
                new GetGarageDoorStatusQuery(TestGarage.Id, TestDoor.Id),
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
                new GetGarageDoorStatusQuery(TestGarage.Id, TestDoor.Id),
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
            .Setup(x => x.CheckStatus(TestDoor))
            .ReturnsAsync(DoorStatus.Open);

        // Act
        await queryHandler.Handle(
            new GetGarageDoorStatusQuery(TestGarage.Id, TestDoor.Id),
            CancellationToken.None);

        // Assert
        this.gatewayMock.Verify(m => m.CheckStatus(TestDoor), Times.Once);
    }

    [Fact]
    public async Task should_return_status_when_gateway_responds()
    {
        // Arrange
        this.gatewayMock
            .Setup(x => x.CheckStatus(TestDoor))
            .ReturnsAsync(DoorStatus.Open);
        var expected = new GetGarageDoorStatusResponse
        {
            Status = DoorStatus.Open,
        };

        // Act
        var actual = await queryHandler.Handle(
            new GetGarageDoorStatusQuery(TestGarage.Id, TestDoor.Id),
            CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
