using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseVersionTool;

internal class VersionManager
{
    private readonly string _connectionString;
    private readonly DbHelper _dbHelper;

    public VersionManager(string connectionString)
    {
        _connectionString = connectionString;
        _dbHelper = new DbHelper(connectionString);
    }

    public IReadOnlyCollection<string> ExecuteMigrations()
    {
        var output = new List<string>();

        // Get current DB version
        var version = GetCurrentVersion();
        output.Add($"Current DB version is {version}");

        // Find new migrations
        var migrations = GetNewMigrations(version);
        output.Add($"{migrations.Count} migrations found");

        // Evecute new migrations
        foreach (var migration in migrations )
        {
            _dbHelper.ExecuteMigration(migration.GetContent());
            UpdateVersion(migration.Version);
            output.Add($"Executed migration {migration.Name}");
        }

        return output;
    }

    private IReadOnlyList<Migration> GetNewMigrations(int version)
    {
        // Example: 01_Migration.sql
        var regex = new Regex(@"^(\d)*_(.*)(sql)$");

        return new DirectoryInfo(@"Migrations\")
            .GetFiles()
            .Where(x => regex.IsMatch(x.Name))
            .Select(x => new Migration(x))
            .Where(x => x.Version > version)
            .OrderBy(x => x.Version)
            .ToList();
    }

    private int GetCurrentVersion()
    {
        if (!SettingsTableExists())
        {
            CreateSettingsTable();
            return 0;
        }
        return GetVersionFromSettingsTable();
    }

    private int GetVersionFromSettingsTable()
    {
        var query = @"SELECT Value from dbo.Settings WHERE Name = 'Version'";
        var version = _dbHelper.ExecuteScalar<string>(query);
        return int.Parse(version);
    }

    private void CreateSettingsTable()
    {
        var query = @"
            CREATE TABLE dbo.Settings
            (
                Name nvarchar(50) NOT NULL PRIMARY KEY,
                Value nvarchar(100) NOT NULL
            )
            INSERT dbo.Settings (Name, Value)
            VALUES ('Version', '0')";

        _dbHelper.ExecuteNonQuery(query);
    }

    private bool SettingsTableExists()
    {
        var query = @"
            IF (OBJECT_ID('dbo.Settings', 'table') IS NULL)
            SELECT 0
            ELSE SELECT 1";

        return _dbHelper.ExecuteScalar<int>(query) == 1;
    }
}
