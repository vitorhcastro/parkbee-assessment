using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ParkingSessions.Commands.CreateParkingSession;

public class
    CreateParkingSessionCommandHandler : IRequestHandler<CreateParkingSessionCommand, CreateParkingSessionResponse>
{
    private readonly IParkingDbContext dbContext;
    private readonly IDoorGateway doorGateway;

    public CreateParkingSessionCommandHandler(IParkingDbContext dbContext, IDoorGateway doorGateway)
    {
        this.dbContext = dbContext;
        this.doorGateway = doorGateway;
    }

    public async Task<CreateParkingSessionResponse> Handle(CreateParkingSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userExists = await this.dbContext.Users.AnyAsync(u => u.Id == request.Request.UserId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException(nameof(User), request.Request.UserId);
        }

        var garageExists =
            await this.dbContext.Garages.AnyAsync(g => g.Id == request.Request.GarageId, cancellationToken);
        if (!garageExists)
        {
            throw new NotFoundException(nameof(Garage), request.Request.GarageId);
        }

        var door = await this.dbContext.Doors.FirstOrDefaultAsync(
            d => d.Id == request.Request.DoorId && d.GarageId == request.Request.GarageId, cancellationToken);
        if (door == null)
        {
            throw new NotFoundException(nameof(Door), request.Request.DoorId);
        }

        var parkingSession = new ParkingSession
        {
            Id = Guid.NewGuid(),
            EntryDoorId = request.Request.DoorId,
            UserId = request.Request.UserId,
            StartDate = DateTime.UtcNow,
        };

        this.dbContext.ParkingSessions.Add(parkingSession);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        await doorGateway.OpenDoor(door);

        return new CreateParkingSessionResponse(parkingSession.Id);
    }
}
