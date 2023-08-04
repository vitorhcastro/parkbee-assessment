using Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ICurrentUserService currentUserService;

    private readonly ILogger logger;

    public LoggingBehaviour(
        ILogger<TRequest> logger,
        ICurrentUserService currentUserService)
    {
        this.logger = logger;
        this.currentUserService = currentUserService;
    }

    public async Task Process(
        TRequest request,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = this.currentUserService.UserId ?? string.Empty;

        this.logger.LogInformation(
            "api Request: {Name} {@UserId} {@Request}",
            requestName,
            userId,
            request);
    }
}
