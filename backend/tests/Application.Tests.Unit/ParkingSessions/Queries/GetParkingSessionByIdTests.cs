using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.ParkingSessions.Queries.GetParkingSessionById;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using static TestHelpers.Builders.ParkingSessionBuilder;

namespace Application.Tests.Unit.ParkingSessions.Queries;

public class GetParkingSessionByIdTests
{
    private static readonly ParkingSession TestParkingSession = AParkingSession().Build();

    private readonly Mock<IParkingDbContext> dbContextMock;
    private readonly GetParkingSessionByIdQueryHandler queryHandler;

    public GetParkingSessionByIdTests()
    {
        dbContextMock = new Mock<IParkingDbContext>();
        queryHandler = new GetParkingSessionByIdQueryHandler(dbContextMock.Object);
    }

    [Fact]
    public async Task should_throw_not_found_exception_when_parking_session_does_not_exist()
    {
        // Arrange
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(new List<ParkingSession> ());

        // Act
        Func<Task> act = async () =>
            await queryHandler.Handle(new GetParkingSessionByIdQuery(TestParkingSession.Id),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(ParkingSession)}\" ({TestParkingSession.Id}) was not found.");
    }

    [Fact]
    public async Task should_return_parking_session_when_exists()
    {
        // Arrange
        var parkingSessions = new List<ParkingSession>
        {
            TestParkingSession,
            AParkingSession().Build(),
            AParkingSession().Build(),
            AParkingSession().Build(),
        };
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(parkingSessions);

        // Act
        var act = await queryHandler.Handle(
            new GetParkingSessionByIdQuery(TestParkingSession.Id),
            CancellationToken.None);

        // Assert
        act.Should().BeEquivalentTo(TestParkingSession);
    }
}
