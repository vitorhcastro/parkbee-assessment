using Application.ParkingSessions.Queries.GetParkingSessionById;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class ParkingSessionsController
{
    private readonly IMediator _mediator;

    public ParkingSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public void CreateParkingSession()
    {
        throw new NotImplementedException();
    }

    [HttpGet("id:guid")]
    public Task<ParkingSession> GetParkingSessionById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetParkingSessionByIdQuery(id);
        return this._mediator.Send(query, cancellationToken);
    }

    [HttpPut("{id:guid}/status")]
    public void UpdateParkingSessionStatus(Guid id)
    {
        throw new NotImplementedException();
    }
}
