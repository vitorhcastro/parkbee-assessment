using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.ParkingSessions.Commands.UpdateParkingSessionStatus;

public class UpdateParkingSessionStatusCommandHandler : IRequestHandler<UpdateParkingSessionStatusCommand>
{
    private readonly IParkingDbContext dbContext;
    private readonly IDoorGateway gateway;

    public UpdateParkingSessionStatusCommandHandler(IParkingDbContext dbContext, IDoorGateway gateway)
    {
        this.dbContext = dbContext;
        this.gateway = gateway;
    }

    public async Task<Unit> Handle(UpdateParkingSessionStatusCommand request, CancellationToken cancellationToken)
    {
        var parkingSession = await this.dbContext.ParkingSessions.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (parkingSession == null)
        {
            throw new NotFoundException(nameof(ParkingSession), request.Id);
        }

        var door = await this.dbContext.Doors.FirstOrDefaultAsync(
            d => d.Id == request.Request.DoorId, cancellationToken);
        if (door == null)
        {
            throw new NotFoundException(nameof(Door), request.Request.DoorId);
        }

        if(parkingSession.Status != ParkingSessionStatus.Running)
        {
            var failures = new List<ValidationFailure>
            {
                new(nameof(parkingSession.Status), "Parking session is not running")
            };
            throw new ValidationException(failures);
        }

        parkingSession.Status = request.Request.Status;
        parkingSession.EndDate = DateTime.UtcNow;
        this.dbContext.ParkingSessions.Update(parkingSession);
        this.dbContext.SetModified(parkingSession);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        await gateway.OpenDoor(door);

        return new Unit();
    }
}
