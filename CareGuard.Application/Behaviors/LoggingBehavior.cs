using MediatR;

namespace CareGuard.Application.Behaviors;

public class LoggingBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse> where TRequest:notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");

        var response = await next();

        Console.WriteLine($"Handled {typeof(TRequest).Name}");

        return response;
    }
}