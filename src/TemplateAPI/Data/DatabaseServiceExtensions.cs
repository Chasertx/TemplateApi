using Microsoft.EntityFrameworkCore;

namespace TemplateAPI.Data;

public static class DatabaseServiceExtensions
{
    /// <summary>
    /// Registers <see cref="AppDbContext"/> using the provider and connection string
    /// from the "Database" configuration section.
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? throw new InvalidOperationException($"Missing '{DatabaseOptions.SectionName}' configuration section.");

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new InvalidOperationException($"'{DatabaseOptions.SectionName}:ConnectionString' is not configured.");

        services.AddDbContext<AppDbContext>(db => _ = options.Provider switch
        {
            DatabaseProvider.Sqlite => db.UseSqlite(options.ConnectionString),
            DatabaseProvider.SqlServer => db.UseSqlServer(options.ConnectionString),
            DatabaseProvider.PostgreSql => db.UseNpgsql(options.ConnectionString),
            _ => throw new InvalidOperationException($"Unsupported database provider '{options.Provider}'.")
        });

        return services;
    }
}
