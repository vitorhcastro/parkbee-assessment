﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace Presentation.Extensions.Configuration;

public static class ControllersExtensions
{
    public static IServiceCollection AddControllersConfiguration(this IServiceCollection services)
    {
        services
            .AddControllers(options => { options.Filters.Add<ApiExceptionFilterAttribute>(); })
            .AddFluentValidation(configuration => { configuration.AutomaticValidationEnabled = false; });

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        return services;
    }
}
