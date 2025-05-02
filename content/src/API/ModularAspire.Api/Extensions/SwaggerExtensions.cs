using Microsoft.OpenApi.Models;

namespace ModularAspire.Api.Extensions;

internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth"}
                    },
                    []
                }
            });
            
            options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ModularAspire API",
                Version = "v1",
                Description = "ModularAspire API built using the modular monolith architecture."
            });

        });
            
        return services;
    }
}
