using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetGarageById;

public class GetGarageByIdQueryHandler : IRequestHandler<GetGarageByIdQuery, GetGarageByIdResponse>
{
    private readonly IParkingDbContext dbContext;

    public GetGarageByIdQueryHandler(
        IParkingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<GetGarageByIdResponse> Handle(GetGarageByIdQuery request, CancellationToken cancellationToken)
    {
        var garage = await this.dbContext.Garages
            .Include(x => x.Doors)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (garage == null)
        {
            throw new NotFoundException(nameof(Garage), request.Id);
        }

        var runningParkingSessions = await this.dbContext.ParkingSessions
            .Where(ps => ps.EntryDoor.GarageId == garage.Id)
            .CountAsync(ps => ps.Status == ParkingSessionStatus.Running, cancellationToken);

        return new GetGarageByIdResponse(garage.Id, garage.Name, garage.TotalSpots, garage.Doors, garage.TotalSpots - runningParkingSessions);
    }
}
