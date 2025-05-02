using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ModularAspire.Modules.Identity.Infrastructure.Middleware;

public class EnrichRegisterRequestMiddleware
{
    private readonly RequestDelegate _next;

    public EnrichRegisterRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Value?.EndsWith("/register") == true && 
            context.Request.Method == "POST")
        {
            context.Request.EnableBuffering();
            
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            
            try
            {
                using var jsonDoc = JsonDocument.Parse(body);
                var root = jsonDoc.RootElement;
                
                if (root.TryGetProperty("firstName", out var firstNameElement) && 
                    root.TryGetProperty("lastName", out var lastNameElement))
                {
                    context.Items["User.FirstName"] = firstNameElement.GetString();
                    context.Items["User.LastName"] = lastNameElement.GetString();
                }
            }
            catch 
            {
            }
        }
        
        await _next(context);
    }
}