using Application.Common.Interfaces;
using Domain.Entities;

namespace Infrastructure.Gateways;

public class DoorStatusGateway : IDoorStatusGateway
{
    public Task<DoorHealth> CheckHealth(Door door)
    {
        throw new NotImplementedException();
    }

    public Task<DoorStatus> CheckStatus(Door door)
    {
        throw new NotImplementedException();
    }
}
