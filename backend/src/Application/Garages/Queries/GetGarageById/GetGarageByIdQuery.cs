using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetGarageById;

public record GetGarageByIdQuery(Guid Id) : IRequest<Garage>;
