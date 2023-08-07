using Application.Garages.Queries.GetAllGarages;
using Application.Garages.Queries.GetGarageById;
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
    public async Task<ActionResult<GarageByIdDto>> GetGarage(Guid garageId, CancellationToken cancellationToken)
    {
        var query = new GetGarageByIdQuery(garageId);
        var garage = await this.mediator.Send(query, cancellationToken);

        return garage;
    }

    [HttpGet("{garageId:guid}/doors/{doorId:guid}/health")]
    public void GetGarageDoorHealthStatus(Guid garageId, Guid doorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{garageId:guid}/doors/{doorId:guid}/status")]
    public void GetGarageDoorStatus(Guid garageId, Guid doorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
