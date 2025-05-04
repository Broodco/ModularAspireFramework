using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularAspire.Common.Application.Authorization;
using ModularAspire.Common.Application.EventBus;
using ModularAspire.Common.Application.Identity;
using ModularAspire.Common.Application.Messaging;
using ModularAspire.Common.Infrastructure.Outbox;
using ModularAspire.Common.Presentation.Authorization;
using ModularAspire.Common.Presentation.Endpoints;
using ModularAspire.Modules.Identity.Application.Abstractions.Data;
using ModularAspire.Modules.Identity.Domain.Users;
using ModularAspire.Modules.Identity.Infrastructure.Authorization;
using ModularAspire.Modules.Identity.Infrastructure.Database;
using ModularAspire.Modules.Identity.Infrastructure.Inbox;
using ModularAspire.Modules.Identity.Infrastructure.Outbox;
using ModularAspire.Modules.Identity.Infrastructure.Users;

namespace ModularAspire.Modules.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services,
        IConfiguration configuration, string connectionString)
    {
        services.AddDomainEventHandlers();
        services.AddIntegrationEventHandlers();
        
        services.AddEndpoints(Presentation.AssemblyReference.Assembly);
        
        services.AddInfrastructure(configuration, connectionString);

        return services;
    }
    
    public static void ConfigureConsumers(IRegistrationConfigurator configurator, string instanceId)
    {
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
    {
        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString,
                npgsqlOptionsAction: ngpsqlOptions => ngpsqlOptions
                    .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Identity)
            );
            options.AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>());
        });

        services.AddIdentityApiEndpoints<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext>();
        
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>()); 
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IModuleAuthorizationService, IdentityAuthorizationService>();
        services.AddSingleton<IAuthorizationHandler, ResourcePermissionHandler>();
        services.AddHttpContextAccessor();

        services.AddScoped<IUserRepository, UserRepository>();
        
        services.Configure<OutboxOptions>(configuration.GetSection("Identity:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        services.Configure<InboxOptions>(configuration.GetSection("Identity:Inbox"));
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