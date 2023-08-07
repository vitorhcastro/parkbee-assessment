using Application.Users.Queries.GetAllUsersByPartnerId;
using Application.Users.Queries.GetUserParkingSessionsById;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator mediator;

    public UsersController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<List<User>> GetUsers(CancellationToken cancellationToken)
    {
        var partnerId = User.Claims.First(x => x.Type == ApplicationConstants.Authentication.PartnerIdCustomClaim).Value;
        var query = new GetAllUsersByPartnerIdQuery(partnerId);

        return await this.mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id:guid}/parking-sessions")]
    public async Task<List<ParkingSession>> GetUserParkingSessionsById(Guid id, CancellationToken cancellationToken, [FromQuery] ParkingSessionStatus? status = null)
    {
        var query = new GetUserParkingSessionsByIdQuery(id, cancellationToken, status);
        var result = await this.mediator.Send(query, cancellationToken);
        return result;
    }
}
