using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModularAspire.Common.Presentation.Authorization;

namespace ModularAspire.Common.Presentation.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        string permission,
        Guid resourceId)
    {
        var requirement = new ResourcePermissionRequirement(permission, resourceId);
        return builder.RequireAuthorization(new AuthorizeAttribute { Policy = $"{permission}-{resourceId}" });
    }
    
    public static RouteHandlerBuilder RequirePermissionFromRoute(
        this RouteHandlerBuilder builder,
        string permission,
        string parameterName = "id")
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            if (!context.HttpContext.Request.RouteValues.TryGetValue(parameterName, out var routeValue) ||
                !Guid.TryParse(routeValue?.ToString(), out var resourceId))
            {
                return Results.BadRequest($"Invalid {parameterName} parameter");
            }
            
            var httpContext = context.HttpContext;
            var authService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var user = httpContext.User;
            
            var authResult = await authService.AuthorizeAsync(
                user, 
                null, 
                new ResourcePermissionRequirement(permission, resourceId));
                
            if (!authResult.Succeeded)
            {
                return Results.Forbid();
            }
            
            return await next(context);
        });
    }
    
    public static RouteHandlerBuilder RequirePermissionFromBody<T>(
        this RouteHandlerBuilder builder,
        string permission,
        Func<T, Guid> shopIdSelector)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var request = context.Arguments.OfType<T>().FirstOrDefault();
            if (request == null)
            {
                return Results.BadRequest("Request body not found");
            }

            var shopId = shopIdSelector(request);
        
            var httpContext = context.HttpContext;
            var authService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var user = httpContext.User;
        
            var authResult = await authService.AuthorizeAsync(
                user, 
                null,
                new IAuthorizationRequirement[] { new ResourcePermissionRequirement(permission, shopId) });
            
            if (!authResult.Succeeded)
            {
                return Results.Forbid();
            }
        
            return await next(context);
        });
    }}