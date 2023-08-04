using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetAllGarages;

public record GetAllGaragesQuery : IRequest<List<Garage>>;
