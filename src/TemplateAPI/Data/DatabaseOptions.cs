namespace TemplateAPI.Data;

public enum DatabaseProvider
{
    Sqlite,
    SqlServer,
    PostgreSql
}

/// <summary>
/// Bound from the "Database" section of configuration (appsettings.json, environment
/// variables such as Database__Provider / Database__ConnectionString, user secrets, etc.).
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "Database";

    public DatabaseProvider Provider { get; set; } = DatabaseProvider.Sqlite;
    public string ConnectionString { get; set; } = string.Empty;
}
