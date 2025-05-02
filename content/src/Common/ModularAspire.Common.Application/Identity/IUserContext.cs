namespace ModularAspire.Common.Application.Identity;

public interface IUserContext
{
    CurrentUser GetCurrentUser();
}