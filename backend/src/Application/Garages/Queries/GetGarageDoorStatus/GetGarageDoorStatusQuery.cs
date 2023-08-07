using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetGarageDoorStatus;

public record GetGarageDoorStatusQuery(Guid GarageId, Guid DoorId) : IRequest<GetGarageDoorStatusResponse>;

public class GetGarageDoorStatusResponse
{
    public DoorStatus Status { get; set; }
}