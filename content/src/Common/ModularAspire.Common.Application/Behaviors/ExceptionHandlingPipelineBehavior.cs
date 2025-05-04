using MediatR;
using Microsoft.Extensions.Logging;
using ModularAspire.Common.Application.Exceptions;
using ApplicationException = ModularAspire.Common.Application.Exceptions.ApplicationException;

namespace ModularAspire.Common.Application.Behaviors;

internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {RequestName}", typeof(TRequest).Name);
            
            throw new ApplicationException(typeof(TRequest).Name, inner: ex);
        }
    }
}