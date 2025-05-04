using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModularAspire.Common.Domain;
using ModularAspire.Modules.Identity.Domain.Users;
using ModularAspire.Modules.Identity.Infrastructure.Database;

namespace ModularAspire.Modules.Identity.Infrastructure.Middleware;

internal sealed class IdentityDomainEventsMiddleware(
    ILogger<IdentityDomainEventsMiddleware> _logger, 
    RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context, IdentityDbContext dbContext)
    {
        var path = context.Request.Path;
        var method = context.Request.Method;
        
        string requestBody = string.Empty;
        if (IsIdentityEndpoint(path) && method == "POST")
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }
        
        await _next(context);
        
        if (IsIdentityEndpoint(path) && context.Response.StatusCode == 200)
        {
            await ProcessIdentityEventAsync(path, method, requestBody, context, dbContext);
        }
    }
    
    private bool IsIdentityEndpoint(PathString path)
    {
        return path.StartsWithSegments("/identity");
    }

    private async Task ProcessIdentityEventAsync(PathString path, string method, string requestBody,
        HttpContext context, IdentityDbContext dbContext)
    {
        try
        {
            if (method == "POST" && path.ToString().EndsWith("/register"))
            {
                var registerRequest = JsonSerializer.Deserialize<RegisterRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (registerRequest != null && !string.IsNullOrEmpty(registerRequest.Email))
                {
                    var user = await dbContext.Users
                        .FirstOrDefaultAsync(u => u.Email == registerRequest.Email);

                    if (user != null && user is IHasDomainEvents eventUser)
                    {
                        eventUser.Raise(new UserRegisteredDomainEvent(user.Id));

                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            if (method == "POST" && path.ToString().EndsWith("/manage/info"))
            {
                var userId = context.User.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await dbContext.Users.FindAsync(userId);
                    if (user != null && user is IHasDomainEvents eventUser)
                    {
                        eventUser.Raise(new UserUpdatedDomainEvent(user.Id));
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing identity event");
        }
    }
}