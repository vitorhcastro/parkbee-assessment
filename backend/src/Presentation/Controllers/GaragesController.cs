using Application.Garages.Queries.GetAllGarages;
using Application.Garages.Queries.GetGarageById;
using Application.Garages.Queries.GetGarageDoorHealth;
using Application.Garages.Queries.GetGarageDoorStatus;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

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

    [HttpGet("{garageId:guid}")]
    public async Task<ActionResult<GetGarageByIdResponse>> GetGarage(Guid garageId, CancellationToken cancellationToken)
    {
        var query = new GetGarageByIdQuery(garageId);
        var garage = await this.mediator.Send(query, cancellationToken);

        return garage;
    }

    [HttpGet("{garageId:guid}/doors/{doorId:guid}/health")]
    public Task<GetGarageDoorHealthResponse> GetGarageDoorHealth(Guid garageId, Guid doorId, CancellationToken cancellationToken)
    {
        var query = new GetGarageDoorHealthQuery(garageId, doorId);
        return this.mediator.Send(query, cancellationToken);
    }

    [HttpGet("{garageId:guid}/doors/{doorId:guid}/status")]
    public Task<GetGarageDoorStatusResponse> GetGarageDoorStatus(Guid garageId, Guid doorId, CancellationToken cancellationToken)
    {
        var query = new GetGarageDoorStatusQuery(garageId, doorId);
        return this.mediator.Send(query, cancellationToken);
    }
}
