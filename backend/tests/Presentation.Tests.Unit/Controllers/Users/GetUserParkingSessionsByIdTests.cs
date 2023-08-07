using Application.Users.Queries.GetUserParkingSessionsById;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Moq;
using Presentation.Controllers;
using static TestHelpers.UserBuilder;
using static TestHelpers.ParkingSessionBuilder;

namespace Presentation.Tests.Unit.Controllers.Users;

public class GetUserParkingSessionsByIdTests
{
    private static readonly User TestUser = AUser().Build();
    private static readonly ParkingSession TestParkingSession = AParkingSession().WithUser(TestUser).Build();
    private readonly Mock<IMediator> mediatorMock;
    private readonly UsersController usersController;

    public GetUserParkingSessionsByIdTests()
    {
        mediatorMock = new Mock<IMediator>();
        usersController = new UsersController(mediatorMock.Object);
    }

    [Fact]
    public async Task should_invoke_mediator_query_without_status()
    {
        // Arrange
        mediatorMock
            .Setup(m => m.Send(new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None, null), default))
            .ReturnsAsync(new List<ParkingSession>());

        // Act
        _ = await usersController.GetUserParkingSessionsById(TestUser.Id, CancellationToken.None);

        // Assert
        mediatorMock.Verify(
            m => m.Send(new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None, null), default),
            Times.Once);
    }

    [Fact]
    public async Task should_invoke_mediator_query_with_status()
    {
        // Arrange
        mediatorMock
            .Setup(m => m.Send(
                new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None, ParkingSessionStatus.Running),
                default))
            .ReturnsAsync(new List<ParkingSession>());

        // Act
        _ = await usersController.GetUserParkingSessionsById(TestUser.Id, CancellationToken.None,
            ParkingSessionStatus.Running);

        // Assert
        mediatorMock.Verify(
            m => m.Send(
                new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None, ParkingSessionStatus.Running),
                default), Times.Once);
    }

    [Fact]
    public async Task should_return_list_from_query()
    {
        // Arrange
        var expected = new List<ParkingSession> { TestParkingSession };
        mediatorMock
            .Setup(m => m.Send(new GetUserParkingSessionsByIdQuery(TestUser.Id, CancellationToken.None, null), default))
            .ReturnsAsync(expected);

        // Act
        var actual = await usersController.GetUserParkingSessionsById(TestUser.Id, CancellationToken.None);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
