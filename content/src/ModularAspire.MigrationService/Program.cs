using ModularAspire.MigrationService;
using ModularAspire.Modules.Identity.Infrastructure.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<IdentityDbContext>("modular-aspire-db");

var host = builder.Build();
host.Run();
