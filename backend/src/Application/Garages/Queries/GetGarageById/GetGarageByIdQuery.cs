using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetGarageById;

public record GetGarageByIdQuery(Guid Id) : IRequest<GetGarageByIdResponse>;

public record GetGarageByIdResponse(Guid Id, string Name, int TotalSpots, List<Door> Doors, int AvailableSpots);
