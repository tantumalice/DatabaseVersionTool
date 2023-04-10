using System.Configuration;

namespace DatabaseVersionTool
{
    internal class Program
    {
        private const string confirmation = "yes";
        static void Main(string[] args)
        {
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["Main"];
            if (string.IsNullOrEmpty(connectionString.ConnectionString))
            {
                Console.WriteLine("There is no connection string with the \"Main\" key");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Type \"yes\" to execute updates");
            if (!string.Equals(Console.ReadLine(), confirmation))
            {
                return;
            }

            var output = new VersionManager(connectionString.ConnectionString).ExecuteMigrations();

            foreach (var line in output)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}