using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Garages.Queries.GetGarageById;

public class GetGarageByIdQueryHandler : IRequestHandler<GetGarageByIdQuery, GarageByIdDto>
{
    private IParkingDbContext parkingDbContext;
    private readonly IMapper mapper;

    public GetGarageByIdQueryHandler(
        IParkingDbContext parkingDbContext,
        IMapper mapper)
    {
        this.parkingDbContext = parkingDbContext;
        this.mapper = mapper;
    }

    public async Task<GarageByIdDto> Handle(GetGarageByIdQuery request, CancellationToken cancellationToken)
    {
        var garage = await this.parkingDbContext.Garages.Include(x => x.Doors)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (garage == null)
        {
            throw new NotFoundException(nameof(Garage), request.Id);
        }

        return this.mapper.Map<GarageByIdDto>(garage);
    }
}
