using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Garages.Queries.GetGarageById;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using static TestHelpers.DoorBuilder;
using static TestHelpers.GarageBuilder;
using static TestHelpers.ParkingSessionBuilder;

namespace Application.Tests.Unit.Garages.Queries;

public class GetGarageByIdQueryTests
{
    private static readonly Door TestDoor = ADoor().Build();
    private static readonly Garage TestGarage = AGarage().WithDoor(TestDoor).Build();

    private readonly Mock<IParkingDbContext> dbContextMock;
    private readonly GetGarageByIdQueryHandler queryHandler;

    public GetGarageByIdQueryTests()
    {
        dbContextMock = new Mock<IParkingDbContext>();
        dbContextMock
            .Setup<DbSet<Garage>>(m => m.Garages)
            .ReturnsDbSet(new List<Garage> { TestGarage });

        queryHandler = new GetGarageByIdQueryHandler(dbContextMock.Object);
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
                new GetGarageByIdQuery(TestGarage.Id),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(Garage)}\" ({TestGarage.Id}) was not found.");
    }

    [Fact]
    public async Task should_calculate_available_spots_based_on_existing_running_parking_sessions()
    {
        // Arrange
        var parkingSessions = new List<ParkingSession>
        {
            AParkingSession().WithEntryDoor(TestDoor).Build(),
            AParkingSession().WithEntryDoor(TestDoor).WithStatus(ParkingSessionStatus.Stopped).Build(),
            AParkingSession().Build(),
            AParkingSession().WithStatus(ParkingSessionStatus.Stopped).Build(),
        };
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(parkingSessions);
        var expectedAvailableSpots = TestGarage.TotalSpots - 1;

        // Act
        var act = await queryHandler.Handle(new GetGarageByIdQuery(TestGarage.Id), CancellationToken.None);

        // Assert
        act.AvailableSpots.Should().Be(expectedAvailableSpots);
    }
}
