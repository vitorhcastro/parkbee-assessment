using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetGarageById;

public class GetGarageByIdQueryHandler : IRequestHandler<GetGarageByIdQuery, Garage>
{
    private IParkingDbContext parkingDbContext;

    public GetGarageByIdQueryHandler(IParkingDbContext parkingDbContext)
    {
        this.parkingDbContext = parkingDbContext;
    }

    public async Task<Garage> Handle(GetGarageByIdQuery request, CancellationToken cancellationToken)
    {
        var garage = await this.parkingDbContext.Garages.Include(x => x.Doors)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (garage == null)
        {
            throw new NotFoundException(nameof(Garage), request.Id);
        }

        return garage;
    }
}
