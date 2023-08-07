using Application.ParkingSessions.Commands.CreateParkingSession;
using Application.ParkingSessions.Queries.GetParkingSessionById;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class ParkingSessionsController
{
    private readonly IMediator mediator;

    public ParkingSessionsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public Task<CreateParkingSessionResponse> CreateParkingSession(CreateParkingSessionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateParkingSessionCommand(request);
        return this.mediator.Send(command, cancellationToken);
    }

    [HttpGet("id:guid")]
    public Task<ParkingSession> GetParkingSessionById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetParkingSessionByIdQuery(id);
        return this.mediator.Send(query, cancellationToken);
    }

    [HttpPut("{id:guid}/status")]
    public void UpdateParkingSessionStatus(Guid id)
    {
        throw new NotImplementedException();
    }
}
