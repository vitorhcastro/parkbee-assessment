using Domain.Entities;
using MediatR;

namespace Application.Users.Queries.GetUserParkingSessionsById;

public record GetUserParkingSessionsByIdQuery(Guid UserId,
    ParkingSessionStatus? ParkingSessionStatus = null) : IRequest<List<ParkingSession>>;
