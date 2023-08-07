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
    public void CreateParkingSession(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id:guid}/status")]
    public void UpdateParkingSessionStatus(Guid id)
    {
        throw new NotImplementedException();
    }
}
