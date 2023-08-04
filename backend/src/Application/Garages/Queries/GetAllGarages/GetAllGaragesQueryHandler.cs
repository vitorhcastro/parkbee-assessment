using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetAllGarages;

public class GetAllGaragesQueryHandler : IRequestHandler<GetAllGaragesQuery, List<Garage>>
{
    private readonly IParkingDbContext parkingDbContext;

    public GetAllGaragesQueryHandler(IParkingDbContext parkingDbContext)
    {
        this.parkingDbContext = parkingDbContext;
    }

    public Task<List<Garage>> Handle(GetAllGaragesQuery request, CancellationToken cancellationToken)
    {
        return this.parkingDbContext.Garages.Include(x => x.Doors).ToListAsync(cancellationToken);
    }
}
