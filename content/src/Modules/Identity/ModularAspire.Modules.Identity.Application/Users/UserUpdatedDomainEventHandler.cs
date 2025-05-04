using MediatR;
using ModularAspire.Common.Application.EventBus;
using ModularAspire.Common.Application.Exceptions;
using ModularAspire.Common.Application.Messaging;
using ModularAspire.Common.Domain;
using ModularAspire.Modules.Identity.Application.Users.GetUser;
using ModularAspire.Modules.Identity.Domain.Users;
using ModularAspire.Modules.Identity.IntegrationEvents;
using ApplicationException = ModularAspire.Common.Application.Exceptions.ApplicationException;

namespace ModularAspire.Modules.Identity.Application.Users;

internal sealed class UserUpdatedDomainEventHandler(ISender sender, IEventBus eventBus)
    : DomainEventHandler<UserUpdatedDomainEvent>
{
    public override async Task Handle(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<User?> result = await sender.Send(new GetUserQuery(domainEvent.UserId), cancellationToken);
        
        if (result.IsFailure)
            throw new ApplicationException(nameof(GetUserQuery), result.Error);

        await eventBus.PublishAsync(
            new UserUpdatedIntegrationEvent(
                Guid.NewGuid(),
                domainEvent.OccuredOnUtc,
                result.Value.Id,
                result.Value.Email), cancellationToken);
    }
}