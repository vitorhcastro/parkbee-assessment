using Domain.Entities;
using MediatR;

namespace Application.Users.Queries.GetAllUsersByPartnerId;

public record GetAllUsersByPartnerIdQuery(string PartnerId) : IRequest<List<User>>;
