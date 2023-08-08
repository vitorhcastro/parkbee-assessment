using System.Net.NetworkInformation;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Infrastructure.Gateways;

public class DoorGateway : IDoorGateway
{
    public DoorHealth CheckHealth(Door door)
    {
        try
        {
            var ping = new Ping();
            var result = ping.Send(door.IpAddress, 1000);
            return result.Status == IPStatus.Success ? DoorHealth.Ok : DoorHealth.Unreachable;
        }
        catch (Exception)
        {
            return DoorHealth.Unreachable;
        }
    }

    public Task<DoorStatus> CheckStatus(Door door)
    {
        return Task.FromResult(DoorStatus.Open);
    }

    public Task OpenDoor(Door door)
    {
        return Task.CompletedTask;
    }
}
