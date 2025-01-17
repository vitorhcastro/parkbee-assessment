﻿using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        // Register known exception types and handlers.
        this.exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationException), this.HandleValidationException },
            { typeof(NotFoundException), this.HandleNotFoundException },
            { typeof(UnauthorizedAccessException), this.HandleUnauthorizedAccessException },
        };
    }

    public override void OnException(ExceptionContext context)
    {
        this.HandleException(context);

        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        var type = context.Exception.GetType();
        if (this.exceptionHandlers.ContainsKey(type))
        {
            this.exceptionHandlers[type].Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            this.HandleInvalidModelStateException(context);
            return;
        }

        this.HandleUnknownException(context);
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;

        var details = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleInvalidModelStateException(ExceptionContext context)
    {
        var details = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message,
        };

        context.Result = new NotFoundObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized,
        };

        context.ExceptionHandled = true;
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };

        context.ExceptionHandled = true;
    }
}
