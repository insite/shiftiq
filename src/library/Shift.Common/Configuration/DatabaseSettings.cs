namespace Shift.Common
{
    public class DatabaseSettings
    {
        public DatabaseConnectionStrings ConnectionStrings { get; set; }
        public bool IsReadOnly { get; set; }

        public DatabaseConnectionSettings Connection { get; set; }
        public string ConnectionString { get; set; }
        public string FileStorage { get; set; }
    }

    public class DatabaseConnectionStrings
    {
        public string Engine { get; set; }
        public string Shift { get; set; }
        public string Temp { get; set; }
    }

    public class DatabaseConnectionSettings
    {
        public const string DefaultHost = "localhost";

        public const string DefaultDatabase = "postgres";

        public const int DefaultPort = 5432;

        public string Host { get; set; } = DefaultHost;

        public int Port { get; set; } = DefaultPort;

        public string Database { get; set; } = DefaultDatabase;

        public string User { get; set; }

        public string Password { get; set; }
    }
}