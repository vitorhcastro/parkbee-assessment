using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetGarageDoorHealth;

public class GetGarageDoorHealthQueryHandler : IRequestHandler<GetGarageDoorHealthQuery,
    GetGarageDoorHealthResponse>
{
    private readonly IParkingDbContext dbContext;
    private readonly IDoorGateway doorGateway;

    public GetGarageDoorHealthQueryHandler(
        IParkingDbContext dbContext,
        IDoorGateway doorGateway)
    {
        this.dbContext = dbContext;
        this.doorGateway = doorGateway;
    }

    public async Task<GetGarageDoorHealthResponse> Handle(GetGarageDoorHealthQuery request,
        CancellationToken cancellationToken)
    {
        var garageExists =
            await this.dbContext.Garages.AnyAsync(u => u.Id == request.GarageId, cancellationToken);
        if (!garageExists)
        {
            throw new NotFoundException(nameof(Garage), request.GarageId);
        }

        var door =
            await this.dbContext.Doors.FirstOrDefaultAsync(u => u.Id == request.DoorId, cancellationToken);
        if (door == null)
        {
            throw new NotFoundException(nameof(Door), request.DoorId);
        }

        var healthStatus = this.doorGateway.CheckHealth(door);

        return new GetGarageDoorHealthResponse
        {
            Health = healthStatus,
        };
    }
}
