using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetGarageDoorHealth;

public record GetGarageDoorHealthQuery(Guid GarageId, Guid DoorId, CancellationToken CancellationToken) : IRequest<GetGarageDoorHealthResponse>;

public class GetGarageDoorHealthResponse
{
    public DoorHealth Health { get; set; }
}
