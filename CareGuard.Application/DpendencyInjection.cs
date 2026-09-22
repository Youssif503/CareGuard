using CareGuard.Application.Behaviors;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace CareGuard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(
                typeof(IAssembliMarker).Assembly);

            options.AddOpenBehavior(
                typeof(ValidationBehavior<,>));

            options.AddOpenBehavior(
                typeof(LoggingBehavior<,>));
        });

        return services;
    }
}