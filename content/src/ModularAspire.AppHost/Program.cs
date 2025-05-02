var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("modular-aspire-cache")
    .WithDataVolume(isReadOnly: false)
    .WithPersistence(
        interval: TimeSpan.FromMinutes(5),
        keysChangedThreshold: 100)
    .WithRedisCommander();

var rabbitMq = builder.AddRabbitMQ("modular-aspire-mq")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("modular-aspire-postgres")
    .WithEnvironment("POSTGRES_DB", "modular-aspire-db")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var modularAspireDb = postgres.AddDatabase("modular-aspire-db");

builder.AddProject<Projects.ModularAspire_Api>("modular-aspire-api")
    .WithReference(redis)
    .WithReference(rabbitMq)
    .WithReference(modularAspireDb)
    .WaitFor(postgres);

builder.AddProject<Projects.ModularAspire_MigrationService>("modular-aspire-migration-service")
    .WithReference(modularAspireDb)
    .WaitFor(postgres);

builder.Build().Run();
