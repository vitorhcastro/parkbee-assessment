using Application.Common.Mappings;
using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetGarageById;

public record GetGarageByIdQuery(Guid Id) : IRequest<GarageByIdDto>;

public record GarageByIdDto(Guid Id, string Name, int TotalSpots, List<Door> Doors, int AvailableSpots) : IMapFrom<Garage>;
