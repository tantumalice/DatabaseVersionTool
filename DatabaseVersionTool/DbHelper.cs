using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DatabaseVersionTool;

internal class DbHelper
{
    private readonly string _connectionString;

    public DbHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public T ExecuteScalar<T>(string commandText, params SqlParameter[] parameters)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            var cmd = new SqlCommand(commandText, conn)
            {
                CommandType = System.Data.CommandType.Text
            };

            foreach (var param in parameters)
            {
                cmd.Parameters.Add(param);
            }

            conn.Open();

            return (T)cmd.ExecuteScalar();
        }

    }

    internal void ExecuteMigration(string commandText)
    {
        var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        var subCommands = regex.Split(commandText);

        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var transaction = conn.BeginTransaction();
            using (var cmd = conn.CreateCommand())
            {
                cmd.Connection = conn;
                cmd.Transaction = transaction;

                foreach (var command in subCommands)
                {
                    if (command.Length <= 0)
                    {
                        continue;
                    }

                    cmd.CommandText = command;
                    cmd.CommandType = CommandType.Text;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            transaction.Commit();
        }
    }

    internal void ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            var cmd = new SqlCommand(commandText, conn)
            {
                CommandType = System.Data.CommandType.Text
            };

            foreach (var param in parameters)
            {
                cmd.Parameters.Add(param);
            }

            conn.Open();

            cmd.ExecuteNonQuery();
        }
    }
}
