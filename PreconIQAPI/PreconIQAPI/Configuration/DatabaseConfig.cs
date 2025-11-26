namespace PreconIQAPI.Configuration;

/// <summary>
/// Represents database configuration settings for the application.
/// </summary>
/// <remarks>
/// This class centralizes all database-related configuration including connection strings,
/// schema settings, and migration configurations.
/// </remarks>
public class DatabaseConfig
{
    /// <summary>
    /// The database schema name for the application tables.
    /// </summary>
    public string Schema { get; set; } = string.Empty;

    /// <summary>
    /// Connection string for local development database.
    /// </summary>
    public string LocalConnection { get; set; } = string.Empty;

    /// <summary>
    /// Connection string for production/default database.
    /// </summary>
    public string DefaultConnection { get; set; } = string.Empty;

    /// <summary>
    /// The name of the migration history table.
    /// </summary>
    public string MigrationsHistoryTable { get; set; } = "__EFMigrationsHistory";

    /// <summary>
    /// The in-memory database name used for development/testing.
    /// </summary>
    public string InMemoryDatabaseName { get; set; } = "BlattnerProjectDashboard";

    /// <summary>
    /// Gets the appropriate connection string based on environment.
    /// </summary>
    /// <param name="isDevelopment">Whether the current environment is development.</param>
    /// <returns>The connection string to use.</returns>
    public string GetConnectionString(bool isDevelopment)
    {
        return isDevelopment ? LocalConnection : DefaultConnection;
    }
}