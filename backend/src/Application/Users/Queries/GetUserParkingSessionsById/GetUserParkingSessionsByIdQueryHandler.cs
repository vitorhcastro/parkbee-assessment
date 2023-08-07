using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserParkingSessionsById;

public class GetUserParkingSessionsByIdQueryHandler : IRequestHandler<GetUserParkingSessionsByIdQuery, List<ParkingSession>>
{
    private readonly IParkingDbContext dbContext;

    public GetUserParkingSessionsByIdQueryHandler(IParkingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<ParkingSession>> Handle(GetUserParkingSessionsByIdQuery request, CancellationToken cancellationToken)
    {
        var userExists = await this.dbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        return await this.dbContext.ParkingSessions
            .Where(ps => ps.UserId == request.UserId)
            .Where(ps => request.ParkingSessionStatus == null || ps.Status == request.ParkingSessionStatus)
            .ToListAsync(cancellationToken);
    }
}
