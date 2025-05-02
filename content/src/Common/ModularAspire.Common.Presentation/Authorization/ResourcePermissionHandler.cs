using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModularAspire.Common.Application.Authorization;

namespace ModularAspire.Common.Presentation.Authorization;

public class ResourcePermissionHandler : IAuthorizationHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ResourcePermissionHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }
        
        var authorizationService = httpContext.RequestServices
            .GetRequiredService<IModuleAuthorizationService>();
            
        foreach (var requirement in context.Requirements.OfType<ResourcePermissionRequirement>())
        {
            var userId = context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
            if (string.IsNullOrEmpty(userId))
            {
                continue;
            }
            
            if (await authorizationService.HasPermissionAsync(
                    userId, requirement.Permission, requirement.ResourceId))
            {
                context.Succeed(requirement);
            }
        }
    }
}