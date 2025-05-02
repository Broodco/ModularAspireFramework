namespace ModularAspire.Modules.Identity.Application.Abstractions.Identity;

public sealed record UserModel(string Email, string Password, string FirstName, string LastName);