using ModularAspire.Common.Domain;

namespace ModularAspire.Modules.Identity.Domain.Users;

public sealed class UserRegisteredDomainEvent : DomainEvent
{
    public UserRegisteredDomainEvent(
        string userId)
    {
        UserId = userId;
    }
    
    public string UserId { get; init; }
}