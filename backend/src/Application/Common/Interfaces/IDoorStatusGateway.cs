using Application.Garages.Queries.GetGarageDoorHealth;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IDoorStatusGateway
{
    Task<DoorHealthStatus> CheckHealth(Door testDoor);
}
