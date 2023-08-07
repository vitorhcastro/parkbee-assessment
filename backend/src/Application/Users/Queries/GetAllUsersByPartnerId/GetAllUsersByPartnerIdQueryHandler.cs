using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetAllUsersByPartnerId;

public class GetAllUsersByPartnerIdQueryHandler : IRequestHandler<GetAllUsersByPartnerIdQuery, List<User>>
{
    private readonly IParkingDbContext dbContext;

    public GetAllUsersByPartnerIdQueryHandler(IParkingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<List<User>> Handle(GetAllUsersByPartnerIdQuery request, CancellationToken cancellationToken)
    {
        return dbContext.Users.Where(x => x.PartnerId == request.PartnerId).ToListAsync(cancellationToken);
    }
}
