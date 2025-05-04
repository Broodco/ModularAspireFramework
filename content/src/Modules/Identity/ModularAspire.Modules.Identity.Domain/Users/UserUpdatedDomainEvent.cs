using ModularAspire.Common.Domain;

namespace ModularAspire.Modules.Identity.Domain.Users;

public sealed class UserUpdatedDomainEvent : DomainEvent
{
    public UserUpdatedDomainEvent(
        string userId)
    {
        UserId = userId;
    }
    
    public string UserId { get; init; }
}