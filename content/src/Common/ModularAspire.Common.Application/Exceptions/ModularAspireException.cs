using ModularAspire.Common.Domain;

namespace ModularAspire.Common.Application.Exceptions;

public sealed class ModularAspireException : Exception
{
    public ModularAspireException(string requestName, Error? error = default, Exception? inner = default)
        : base("Application exception", inner)
    {
        RequestName = requestName;
        Error = error;
    }
    
    public string RequestName { get; }
    public Error? Error { get; }
}