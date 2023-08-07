using MediatR;

namespace Application.Garages.Queries.GetGarageDoorHealth;

public record GetGarageDoorHealthQuery(Guid GarageId, Guid DoorId, CancellationToken CancellationToken) : IRequest<GetGarageDoorHealthResponse>;

public class GetGarageDoorHealthResponse
{
    public DoorHealthStatus Health { get; set; }
}

public enum DoorHealthStatus
{
    Unknown = 0,
    Ok = 1,
    Unreachable = 2,
}
