using Microsoft.EntityFrameworkCore;
using ModularAspire.Common.Infrastructure.Inbox;
using ModularAspire.Common.Infrastructure.Outbox;
using ModularAspire.Modules.ModuleName.Application.Abstractions.Data;

namespace ModularAspire.Modules.ModuleName.Infrastructure.Database;

public sealed class ModuleNameDbContext(DbContextOptions<ModuleNameDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.ModuleName);
        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
    }
}