using Domain.Entities;
using MediatR;

namespace Application.ParkingSessions.Queries.GetParkingSessionById;

public record GetParkingSessionByIdQuery(Guid Id) : IRequest<ParkingSession>;
