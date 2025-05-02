using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularAspire.Common.Application.Caching;
using ModularAspire.Common.Application.Clock;
using ModularAspire.Common.Application.Data;
using ModularAspire.Common.Application.EventBus;
using ModularAspire.Common.Infrastructure.Caching;
using ModularAspire.Common.Infrastructure.Clock;
using ModularAspire.Common.Infrastructure.Data;
using ModularAspire.Common.Infrastructure.Outbox;
using Npgsql;
using Quartz;
using StackExchange.Redis;

namespace ModularAspire.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator, string>[] moduleConfigureConsumers,
        string databaseConnectionString,
        string cacheConnectionString,
        string rabbitMqConnectionString)
    {
        var npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);
        
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddQuartz();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.TryAddSingleton<ICacheService, CacheService>();
        IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(cacheConnectionString);
        services.TryAddSingleton(connectionMultiplexer);
        services.AddStackExchangeRedisCache(options => options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        
        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();
        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        services.AddMassTransit(configure =>
        {
            string instanceId = serviceName.ToLowerInvariant().Replace(".", "-");
            
            foreach (Action<IRegistrationConfigurator, string> configureConsumer in moduleConfigureConsumers)
            {
                configureConsumer(configure, instanceId);
            }
            
            configure.SetKebabCaseEndpointNameFormatter();
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConnectionString);
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}