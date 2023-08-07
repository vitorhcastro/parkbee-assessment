using Domain.Entities;
using MediatR;

namespace Application.ParkingSessions.Commands.UpdateParkingSessionStatus;

public record UpdateParkingSessionStatusCommand(Guid Id, UpdateParkingSessionStatusRequest Request) : IRequest;

public record UpdateParkingSessionStatusRequest(Guid DoorId, ParkingSessionStatus Status);
