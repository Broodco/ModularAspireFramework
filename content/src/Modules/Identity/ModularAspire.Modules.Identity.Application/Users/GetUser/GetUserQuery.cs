using ModularAspire.Common.Application.Messaging;
using ModularAspire.Modules.Identity.Domain.Users;

namespace ModularAspire.Modules.Identity.Application.Users.GetUser;

public sealed record GetUserQuery(string UserId) : IQuery<User?>;