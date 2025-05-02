using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModularAspire.Common.Infrastructure.Inbox;

public sealed class InboxMessageConsumerConfiguration : IEntityTypeConfiguration<InboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<InboxMessageConsumer> builder)
    {
        builder.ToTable("InboxMessageConsumers");

        builder.HasKey(o => new { o.InboxMessageId, o.Name });

        builder.Property(o => o.Name).HasMaxLength(500);
    }
}
