using CareGuard.Application.Common;
using FluentValidation;
using MediatR;

public class ValidationBehavior<TRequest, T>
    : IPipelineBehavior<TRequest, Result<T>>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<T>> Handle(
        TRequest request,
        RequestHandlerDelegate<Result<T>> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, cancellationToken))
        );

        var errors = results
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .Select(x => x.ErrorMessage)
            .Distinct()
            .ToArray();

        if (errors.Length > 0)
        {
            return Result<T>.Failure(errors);
        }

        return await next();
    }
}