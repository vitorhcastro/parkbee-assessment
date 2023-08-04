using Application.Garages.Queries.GetAllGarages;
using Application.Garages.Queries.GetGarageById;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GaragesController : ControllerBase
{
    private readonly IMediator mediator;

    public GaragesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public Task<List<Garage>> GetGarages(CancellationToken cancellationToken)
    {
        return this.mediator.Send(new GetAllGaragesQuery(), cancellationToken);
    }

    [HttpGet("{garageId}")]
    public async Task<ActionResult<Garage>> GetGarage(Guid garageId, CancellationToken cancellationToken)
    {
        var query = new GetGarageByIdQuery(garageId);
        var garage = await this.mediator.Send(query, cancellationToken);

        return garage;
    }
}
