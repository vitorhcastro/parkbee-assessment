using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Queries.GetUserParkingSessionsById;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Application.Tests.Unit.Users.Queries;

using static TestHelpers.UserBuilder;
using static TestHelpers.ParkingSessionBuilder;

public class GetUserParkingSessionsByIdTests
{
    private static readonly User TestUser = AUser().Build();
    private static readonly ParkingSession TestParkingSession = AParkingSession().WithUser(TestUser).Build();

    private readonly Mock<IParkingDbContext> dbContextMock;
    private readonly GetUserParkingSessionsByIdQueryHandler queryHandler;

    public GetUserParkingSessionsByIdTests()
    {
        dbContextMock = new Mock<IParkingDbContext>();
        dbContextMock
            .Setup<DbSet<User>>(m => m.Users)
            .ReturnsDbSet(new List<User> { TestUser });
        queryHandler = new GetUserParkingSessionsByIdQueryHandler(dbContextMock.Object);
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
            await queryHandler.Handle(new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None),
                CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"{nameof(User)}\" ({TestUser.Id}) was not found.");
    }

    [Fact]
    public async Task should_return_all_parking_sessions_when_user_exists()
    {
        // Arrange
        var expected = new List<ParkingSession> { TestParkingSession };
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(expected);

        // Act
        var act = await queryHandler.Handle(new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None),
            CancellationToken.None);

        // Assert
        act.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task should_return_filtered_parking_sessions_when_filter_is_passed()
    {
        // Arrange
        var parkingSessions = new List<ParkingSession>
        {
            TestParkingSession,
            AParkingSession().WithUser(TestUser).WithStatus(ParkingSessionStatus.Stopped).Build(),
            AParkingSession().WithStatus(ParkingSessionStatus.Stopped).Build(),
            AParkingSession().WithStatus(ParkingSessionStatus.Running).Build(),
        };
        dbContextMock
            .Setup<DbSet<ParkingSession>>(m => m.ParkingSessions)
            .ReturnsDbSet(parkingSessions);

        var expected = new List<ParkingSession> { TestParkingSession };

        // Act
        var act = await queryHandler.Handle(
            new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None, ParkingSessionStatus.Running),
            CancellationToken.None);

        // Assert
        act.Should().BeEquivalentTo(expected);
    }
}
