using System.Data.SqlClient;

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
