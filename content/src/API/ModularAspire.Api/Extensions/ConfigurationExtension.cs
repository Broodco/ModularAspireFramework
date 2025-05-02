using ModularAspire.Modules.Identity.Infrastructure.Database;

namespace ModularAspire.Api.Extensions;

internal static class ConfigurationExtension
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder builder, string[] modules)
    {
        foreach (var module in modules)
        {
            builder.AddJsonFile($"modules.{module}.json", false, true);
            builder.AddJsonFile($"modules.{module}.Development.json", true, true);
        }
    }

    internal static void EnrichDatabaseContexts(this WebApplicationBuilder builder)
    {
        builder.EnrichNpgsqlDbContext<IdentityDbContext>();
    }
}