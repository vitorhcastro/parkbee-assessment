using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ParkingSessions.Queries.GetParkingSessionById;

public class GetParkingSessionByIdQueryHandler : IRequestHandler<GetParkingSessionByIdQuery, ParkingSession>
{
    private readonly IParkingDbContext dbContext;

    public GetParkingSessionByIdQueryHandler(IParkingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ParkingSession> Handle(GetParkingSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var parkingSession = await this.dbContext.ParkingSessions.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (parkingSession == null)
        {
            throw new NotFoundException(nameof(ParkingSession), request.Id);
        }

        return parkingSession;
    }
}
