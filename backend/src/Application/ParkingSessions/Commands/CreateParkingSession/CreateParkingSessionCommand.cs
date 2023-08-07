using MediatR;

namespace Application.ParkingSessions.Commands.CreateParkingSession;

public record CreateParkingSessionCommand(CreateParkingSessionRequest Request) : IRequest<CreateParkingSessionResponse>;

public record CreateParkingSessionRequest(Guid UserId, Guid GarageId, Guid DoorId);

public record CreateParkingSessionResponse(Guid Id);
