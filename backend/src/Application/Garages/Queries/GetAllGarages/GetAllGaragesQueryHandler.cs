using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetAllGarages;

public class GetAllGaragesQueryHandler : IRequestHandler<GetAllGaragesQuery, List<Garage>>
{
    private readonly IParkingDbContext dbContext;

    public GetAllGaragesQueryHandler(IParkingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<List<Garage>> Handle(GetAllGaragesQuery request, CancellationToken cancellationToken)
    {
        return this.dbContext.Garages.Include(x => x.Doors).ToListAsync(cancellationToken);
    }
}
