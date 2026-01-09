namespace InSite.Persistence
{
    public static class DbSettings
    {
        internal static string ConnectionString { get; private set; }
        internal static string Domain { get; private set; }
        internal static bool IsReadOnly { get; private set; }

        public static void Init(string connectionString, string domain, bool isReadOnly)
        {
            ConnectionString = connectionString;
            Domain = domain;
            IsReadOnly = isReadOnly;
        }
    }
}
