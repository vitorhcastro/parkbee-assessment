using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IDoorStatusGateway
{
    Task<DoorHealth> CheckHealth(Door door);
    Task<DoorStatus> CheckStatus(Door door);
}
