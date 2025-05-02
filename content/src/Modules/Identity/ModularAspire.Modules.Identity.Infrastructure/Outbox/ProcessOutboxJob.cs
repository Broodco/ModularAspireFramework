using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularAspire.Common.Application.Clock;
using ModularAspire.Common.Application.Data;
using ModularAspire.Common.Application.Messaging;
using ModularAspire.Common.Domain;
using ModularAspire.Common.Infrastructure.Outbox;
using ModularAspire.Common.Infrastructure.Serialization;
using Newtonsoft.Json;
using Quartz;

namespace ModularAspire.Modules.Identity.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxJob> logger) : IJob
{
    private const string ModuleName = "Identity";
    
    public async Task Execute(IJobExecutionContext context)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();
        
        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    SerializerSettings.Instance)!;
                
                using IServiceScope scope = serviceScopeFactory.CreateScope();

                var domainEventHandlers = DomainEventHandlersFactory.GetHandlers(
                    domainEvent.GetType(),
                    scope.ServiceProvider,
                    Application.AssemblyReference.Assembly);

                foreach (IDomainEventHandler domainEventHandler in domainEventHandlers)
                {
                    await domainEventHandler.Handle(domainEvent);
                }
            }
            catch (Exception caughtException)
            {
                logger.LogError(caughtException, "{Module} - Failed processing outbox message {MessageId}", ModuleName, outboxMessage.Id);
                
                exception = caughtException;
            }
            
            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }
        
        await transaction.CommitAsync();
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql =
            $"""
             SELECT
                "Id" AS {nameof(OutboxMessageResponse.Id)},
                "Content" AS {nameof(OutboxMessageResponse.Content)}
             FROM "Identity"."OutboxMessages"
             WHERE "ProcessedOnUtc" IS NULL
             ORDER BY "OccuredOnUtc"
             LIMIT {outboxOptions.Value.BatchSize}
             FOR UPDATE
             """;
        
        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);
        
        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql =
            """
            UPDATE "Identity"."OutboxMessages"
            SET "ProcessedOnUtc" = @ProcessedOnUtc,
                "Error" = @Error
            WHERE "Id" = @Id
            """;
        
        await connection.ExecuteAsync(sql, new
        {
            Id = outboxMessage.Id,
            ProcessedOnUtc = dateTimeProvider.UtcNow,
            Error = exception?.ToString()
        },
            transaction: transaction);
    }
}