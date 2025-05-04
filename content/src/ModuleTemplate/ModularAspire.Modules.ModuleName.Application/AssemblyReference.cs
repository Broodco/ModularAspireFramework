using System.Reflection;

namespace ModularAspire.Modules.ModuleName.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
