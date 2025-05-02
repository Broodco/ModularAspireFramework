using MediatR;
using ModularAspire.Common.Domain;

namespace ModularAspire.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
