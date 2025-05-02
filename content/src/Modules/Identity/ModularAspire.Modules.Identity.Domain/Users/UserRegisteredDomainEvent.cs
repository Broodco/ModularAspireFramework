using ModularAspire.Common.Domain;

namespace ModularAspire.Modules.Identity.Domain.Users;

public sealed class UserRegisteredDomainEvent : DomainEvent
{
    public UserRegisteredDomainEvent(
        Guid userId,
        string email,
        string firstName,
        string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
    
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
}