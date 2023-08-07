using Application.Common.Interfaces;
using Domain.Entities;

namespace Infrastructure.Gateways;

public class DoorGateway : IDoorGateway
{
    public Task<DoorHealth> CheckHealth(Door door)
    {
        throw new NotImplementedException();
    }

    public Task<DoorStatus> CheckStatus(Door door)
    {
        throw new NotImplementedException();
    }

    public Task OpenDoor(Door door)
    {
        throw new NotImplementedException();
    }
}
