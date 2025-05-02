using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ModularAspire.Common.Presentation.Authorization;

namespace ModularAspire.Api.Providers;

public class DynamicPermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;
    private readonly IOptions<AuthorizationOptions> _options;
    
    public DynamicPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _options = options;
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }
    
    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        var policy = _options.Value.GetPolicy(policyName);
        if (policy != null)
        {
            return Task.FromResult(policy);
        }
        
        var parts = policyName.Split('-');
        if (parts.Length == 2 && Guid.TryParse(parts[1], out var resourceId))
        {
            var permission = parts[0];
            var dynamicPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new ResourcePermissionRequirement(permission, resourceId))
                .Build();
                
            return Task.FromResult(dynamicPolicy);
        }
        
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
    
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => 
        _fallbackPolicyProvider.GetDefaultPolicyAsync();
        
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => 
        _fallbackPolicyProvider.GetFallbackPolicyAsync();
}