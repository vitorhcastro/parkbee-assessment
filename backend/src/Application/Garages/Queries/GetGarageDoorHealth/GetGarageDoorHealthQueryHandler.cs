using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetGarageDoorHealth;

public class
    GetGarageDoorHealthQueryHandler : IRequestHandler<GetGarageDoorHealthQuery,
        GetGarageDoorHealthResponse>
{
    private readonly IParkingDbContext dbContext;
    private readonly IDoorStatusGateway doorStatusGateway;

    public GetGarageDoorHealthQueryHandler(
        IParkingDbContext dbContext,
        IDoorStatusGateway doorStatusGateway)
    {
        this.dbContext = dbContext;
        this.doorStatusGateway = doorStatusGateway;
    }

    public async Task<GetGarageDoorHealthResponse> Handle(GetGarageDoorHealthQuery request,
        CancellationToken cancellationToken)
    {
        var garageExists =
            await this.dbContext.Garages.AnyAsync(u => u.Id == request.GarageId, request.CancellationToken);
        if (!garageExists)
        {
            throw new NotFoundException(nameof(Garage), request.GarageId);
        }

        var door =
            await this.dbContext.Doors.FirstOrDefaultAsync(u => u.Id == request.DoorId, request.CancellationToken);
        if (door == null)
        {
            throw new NotFoundException(nameof(Door), request.DoorId);
        }

        var healthStatus = await this.doorStatusGateway.CheckHealth(door);

        return new GetGarageDoorHealthResponse
        {
            Health = healthStatus,
        };
    }
}
