using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IDoorGateway
{
    DoorHealth CheckHealth(Door door);
    Task<DoorStatus> CheckStatus(Door door);
    Task OpenDoor(Door door);
}
