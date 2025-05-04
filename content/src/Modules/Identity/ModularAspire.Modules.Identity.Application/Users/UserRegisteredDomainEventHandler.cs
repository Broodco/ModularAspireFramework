using MediatR;
using ModularAspire.Common.Application.EventBus;
using ModularAspire.Common.Application.Exceptions;
using ModularAspire.Common.Application.Messaging;
using ModularAspire.Common.Domain;
using ModularAspire.Modules.Identity.Application.Users.GetUser;
using ModularAspire.Modules.Identity.Domain.Users;
using ModularAspire.Modules.Identity.IntegrationEvents;

namespace ModularAspire.Modules.Identity.Application.Users;

internal sealed class UserRegisteredDomainEventHandler(ISender sender, IEventBus eventBus)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<User?> result = await sender.Send(new GetUserQuery(domainEvent.UserId), cancellationToken);
        
        if (result.IsFailure)
            throw new ModularAspireException(nameof(GetUserQuery), result.Error);

        await eventBus.PublishAsync(
            new UserCreatedIntegrationEvent(
                Guid.NewGuid(),
                domainEvent.OccuredOnUtc,
                result.Value.Id,
                result.Value.Email), cancellationToken);
    }
}