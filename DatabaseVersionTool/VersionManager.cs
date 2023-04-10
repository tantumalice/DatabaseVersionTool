using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseVersionTool;

internal class VersionManager
{
    private readonly string _connectionString;

    public VersionManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IReadOnlyCollection<string> ExecuteMigrations()
    {
        // Get current DB version or generate one

        // Find new migrations

        // Evecute new migrations
    }
}
