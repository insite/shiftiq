using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Microsoft.Data.SqlClient;

using Shift.Common;

namespace Shift.Test;

internal static class TestInitializer
{
    private static string TestDatabaseName = "test";

    /// <summary>
    /// The last SQL upgrade script executed on the database before CreateDatabaseObjects.sql was generated
    /// </summary>
    private static string LastUpgradeScript = "257.2025.1215.1518 TEC-239.sql";

    /// <summary>
    /// The last upgrade maintenance command executed on the database before CreateDatabaseObjects.sql was generated
    /// </summary>
    // private static string LastUpgradeAction = "Upgrade_257_2025_1210_2211"; <== Not needed yet, but maybe in future

    private static AppSettings Settings;

    private static string TempConnectionString;

    private static string TestConnectionString;

    static TestInitializer()
    {
        Settings = AppSettingsHelper.GetAllSettings<AppSettings>();

        TempConnectionString = Settings.Database.ConnectionStrings.Shift;

        TestConnectionString = CreateTestDatabaseConnectionString();
    }

    [ModuleInitializer]
    internal static void Initialize()
    {
        SetupFactories();

        DropAndCreateDatabase();

        CreateDatabaseObjects();

        UpgradeDatabaseObjects();
    }

    private static void SetupFactories()
    {
        UuidFactory.NamespaceId = UuidFactory.CreateV5ForUrl("Shift.Test");
    }

    private static void DropAndCreateDatabase()
    {
        Console.WriteLine($"Creating new database in SQL Server: {TestDatabaseName}");

        using var connection = new SqlConnection(TempConnectionString);

        connection.Open();

        // If the test database already exists, then drop it.

        var dropTable = $"IF DB_ID('{TestDatabaseName}') IS NOT NULL ALTER DATABASE [{TestDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE IF EXISTS [{TestDatabaseName}]; CREATE DATABASE [{TestDatabaseName}];";

        using (var dropCommand = new SqlCommand(dropTable, connection))
        {
            dropCommand.ExecuteNonQuery();
        }
    }

    private static void CreateDatabaseObjects()
    {
        Console.WriteLine($"Creating SQL database objects in {TestDatabaseName}");

        using var connection = new SqlConnection(TestConnectionString);

        connection.Open();

        var scriptPath = Path.Combine(AppContext.BaseDirectory, "Metadata", "CreateDatabaseObjects.sql");

        var script = File.ReadAllText(scriptPath);

        var batches = SplitOnGo(script);

        foreach (var batch in batches)
        {
            if (string.IsNullOrWhiteSpace(batch))
                continue;

            using var command = new SqlCommand(batch, connection);

            command.ExecuteNonQuery();
        }
    }

    private static void UpgradeDatabaseObjects()
    {
        var scriptsPath = Path.Combine(AppContext.BaseDirectory, "Metadata", "Upgrades", "Scripts");

        var scriptFiles = Directory.GetFiles(scriptsPath, "*.sql", SearchOption.AllDirectories)
            .Where(f => string.Compare(Path.GetFileName(f), LastUpgradeScript, StringComparison.OrdinalIgnoreCase) > 0)
            .OrderBy(f => Path.GetFileName(f))
            .ToArray();

        using var connection = new SqlConnection(TestConnectionString);

        connection.Open();

        foreach (var scriptFile in scriptFiles)
        {
            Console.WriteLine($"Executing SQL upgrade script {Path.GetFileName(scriptFile)} on {TestDatabaseName}");

            var script = File.ReadAllText(scriptFile);

            var batches = SplitOnGo(script);

            foreach (var batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch))
                    continue;

                using var command = new SqlCommand(batch, connection);

                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Splits the input script on GO statements (case-insensitive, must be on their own line)
    /// </summary>
    private static IEnumerable<string> SplitOnGo(string script)
    {
        return Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                    .Where(batch => !string.IsNullOrWhiteSpace(batch));
    }

    private static string CreateTestDatabaseConnectionString()
    {
        var csb = new SqlConnectionStringBuilder(TempConnectionString);

        csb.InitialCatalog = TestDatabaseName;

        return csb.ConnectionString;
    }
}
