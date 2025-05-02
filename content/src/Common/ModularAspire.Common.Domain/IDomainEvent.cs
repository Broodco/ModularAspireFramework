using MediatR;

namespace ModularAspire.Common.Domain;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTime OccuredOnUtc { get; }
}