using Microsoft.AspNetCore.Identity;
using ModularAspire.Common.Domain;

namespace ModularAspire.Modules.Identity.Domain.Users;

public sealed class User : IdentityUser, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}