using System.Data.Common;
using Dapper;
using MassTransit;
using ModularAspire.Common.Application.Data;
using ModularAspire.Common.Application.EventBus;
using ModularAspire.Common.Infrastructure.Inbox;
using ModularAspire.Common.Infrastructure.Serialization;
using Newtonsoft.Json;

namespace ModularAspire.Modules.ModuleName.Infrastructure.Inbox;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(IDbConnectionFactory dbConnectionFactory)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        TIntegrationEvent integrationEvent = context.Message;

        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.Instance),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        };

        const string sql =
            """
            INSERT INTO "ModuleName"."InboxMessages"("Id", "Type", "Content", "OccurredOnUtc")
            VALUES (@Id, @Type, @Content::json, @OccurredOnUtc)
            """;

        await connection.ExecuteAsync(sql, inboxMessage);
    }
}
