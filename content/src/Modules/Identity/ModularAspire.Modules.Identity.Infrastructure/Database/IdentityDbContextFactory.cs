using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ModularAspire.Modules.Identity.Infrastructure.Database;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        // Locate configuration file for startup
        var basePath = Directory.GetCurrentDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Add appsettings.json
            .AddEnvironmentVariables() // Fallback to environment variables
            .Build();

        // Retrieve the connection string
        var connectionString = "Host=localhost;Port=5432;Database=shopconnect-db;Username=postgres;Password=password";

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("The connection string 'shopconnect-db' is missing in appsettings.json or environment variables.");
        }
            
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql(connectionString); // Use Npgsql for PostgreSQL

        return new IdentityDbContext(optionsBuilder.Options);
    }
}
