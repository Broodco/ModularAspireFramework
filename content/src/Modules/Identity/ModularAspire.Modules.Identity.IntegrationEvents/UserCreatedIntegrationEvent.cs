using ModularAspire.Common.Application.EventBus;

namespace ModularAspire.Modules.Identity.IntegrationEvents;

public sealed class UserCreatedIntegrationEvent(Guid id, DateTime occurredOnUtc, string userId, string? email)
    : IntegrationEvent(id, occurredOnUtc)
{
    public string UserId { get; init; } = userId;
    public string? Email { get; init; } = email;
}