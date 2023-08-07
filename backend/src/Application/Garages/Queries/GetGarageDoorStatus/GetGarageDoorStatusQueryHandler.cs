using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetGarageDoorStatus;

public class GetGarageDoorStatusQueryHandler : IRequestHandler<GetGarageDoorStatusQuery,
    GetGarageDoorStatusResponse>
{
    private readonly IParkingDbContext dbContext;
    private readonly IDoorStatusGateway doorStatusGateway;

    public GetGarageDoorStatusQueryHandler(
        IParkingDbContext dbContext,
        IDoorStatusGateway doorStatusGateway)
    {
        this.dbContext = dbContext;
        this.doorStatusGateway = doorStatusGateway;
    }

    public async Task<GetGarageDoorStatusResponse> Handle(GetGarageDoorStatusQuery request,
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

        var status = await this.doorStatusGateway.CheckStatus(door);

        return new GetGarageDoorStatusResponse
        {
            Status = status,
        };
    }
}
