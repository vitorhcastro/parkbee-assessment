using Domain.Entities;
using FluentAssertions;
using Infrastructure.Gateways;
using static TestHelpers.Builders.DoorBuilder;

namespace Infrastructure.Tests.Unit.Gateway.Door;

public class CheckHealthTests
{
    private readonly DoorGateway doorGateway = new();

    [Fact]
    public void should_return_ok_when_pinging_parkbee_dot_com()
    {
        // Arrange
        var testDoor = ADoor().WithIpAddress("parkbee.com").Build();

        // Act
        var result = doorGateway.CheckHealth(testDoor);

        // Assert
        result.Should().Be(DoorHealth.Ok);
    }
}
