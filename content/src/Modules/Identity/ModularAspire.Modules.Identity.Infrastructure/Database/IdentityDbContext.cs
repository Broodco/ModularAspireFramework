using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModularAspire.Common.Infrastructure.Inbox;
using ModularAspire.Common.Infrastructure.Outbox;
using ModularAspire.Modules.Identity.Application.Abstractions.Data;
using ModularAspire.Modules.Identity.Domain.Users;

namespace ModularAspire.Modules.Identity.Infrastructure.Database;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) 
    : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<User, IdentityRole, string>(options), IUnitOfWork
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(Schemas.Identity);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
    }
}