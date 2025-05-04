using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ModularAspire.Api.Extensions;
using ModularAspire.Api.Middlewares;
using ModularAspire.Api.Providers;
using ModularAspire.Common.Application;
using ModularAspire.Common.Infrastructure;
using ModularAspire.Common.Presentation.Endpoints;
using ModularAspire.Modules.Identity.Domain.Users;
using ModularAspire.Modules.Identity.Infrastructure;
using ModularAspire.Modules.Identity.Infrastructure.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogExtension();

builder.AddServiceDefaults();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
    
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)    
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("JwtSettings", options))
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("CookieSettings", options));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

builder.Services.AddApplication([
    ModularAspire.Modules.Identity.Application.AssemblyReference.Assembly
]);

builder.Services.AddInfrastructure(
    "ModularAspire.Api",
    [IdentityModule.ConfigureConsumers],
    builder.Configuration.GetConnectionString("modular-aspire-db")!,
    builder.Configuration.GetConnectionString("modular-aspire-cache")!,
    builder.Configuration.GetConnectionString("modular-aspire-mq")!);

builder.Configuration.AddModuleConfiguration(["identity"]);

builder.Services.AddIdentityModule(builder.Configuration, builder.Configuration.GetConnectionString("modular-aspire-db")!);

builder.EnrichDatabaseContexts();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIdentityDomainEvents();

app.MapGroup("identity/")
    .MapIdentityApi<User>()
    .WithTags("Users");

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.Run();
