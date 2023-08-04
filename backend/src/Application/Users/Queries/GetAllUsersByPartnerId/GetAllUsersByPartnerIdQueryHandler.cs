using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetAllUsersByPartnerId;

public class GetAllUsersByPartnerIdQueryHandler : IRequestHandler<GetAllUsersByPartnerIdQuery, List<User>>
{
    private IParkingDbContext parkingDbContext;

    public GetAllUsersByPartnerIdQueryHandler(IParkingDbContext parkingDbContext)
    {
        this.parkingDbContext = parkingDbContext;
    }

    public Task<List<User>> Handle(GetAllUsersByPartnerIdQuery request, CancellationToken cancellationToken)
    {
        return parkingDbContext.Users.Where(x => x.PartnerId == request.PartnerId).ToListAsync(cancellationToken);
    }
}
