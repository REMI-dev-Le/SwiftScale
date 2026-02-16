
using FluentValidation;
using MediatR;
using SwiftScale.BuildingBlocks.Exceptions;
namespace SwiftScale.BuildingBlocks.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                // For now, we throw an exception. 
                // In Step 4, we will catch this and return a clean 400 Bad Request.
                throw new CustomValidationException(failures.Select(f =>new ValidationError(f.PropertyName, f.ErrorMessage)));
            }

            return await next();
        }
    }
}
