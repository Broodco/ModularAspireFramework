using System.Net.Mime;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularAspire.Common.Application.EventBus;
using ModularAspire.Common.Application.Messaging;
using ModularAspire.Common.Infrastructure.Outbox;
using ModularAspire.Common.Presentation.Endpoints;
using ModularAspire.Modules.ModuleName.Application.Abstractions.Data;
using ModularAspire.Modules.ModuleName.Infrastructure.Database;
using ModularAspire.Modules.ModuleName.Infrastructure.Inbox;
using ModularAspire.Modules.ModuleName.Infrastructure.Outbox;

namespace ModularAspire.Modules.ModuleName.Infrastructure;

public static class ModuleNameModule
{
    public static IServiceCollection AddModuleNameModule(this IServiceCollection services,
        IConfiguration configuration, string connectionString)
    {
        services.AddDomainEventHandlers();
        services.AddIntegrationEventHandlers();
        
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);
        
        services.AddInfrastructure(configuration, connectionString);

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator configurator)
    {
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
    {
        services.AddDbContext<ModuleNameDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString,
                npgsqlOptionsAction: ngpsqlOptions => ngpsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.ModuleName)
            );
            options.AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ModuleNameDbContext>()); 
        
        services.Configure<OutboxOptions>(configuration.GetSection("ModuleName:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        services.Configure<InboxOptions>(configuration.GetSection("ModuleName:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob>();
    }
    
    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);
            
            Type domainEvent = domainEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }
    
    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.TryAddScoped(integrationEventHandler);

            Type integrationEvent = integrationEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler =
                typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

            services.Decorate(integrationEventHandler, closedIdempotentHandler);
        }
    }
}
