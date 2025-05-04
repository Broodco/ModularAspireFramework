using Microsoft.AspNetCore.Builder;

namespace ModularAspire.Modules.Identity.Infrastructure.Middleware;

public static class IdentityDomainEventsMiddlewareExtensions
{
    public static IApplicationBuilder UseIdentityDomainEvents(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IdentityDomainEventsMiddleware>();
    }
}