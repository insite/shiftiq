namespace Shift.Test
{
    public static class AppSettingsHelper
    {
        public static T GetAllSettings<T>(string filename) where T : new()
        {
            var configuration = BuildConfiguration(filename);

            var settings = configuration.Get<T>();

            return settings ?? new T();
        }

        private static IConfigurationRoot BuildConfiguration(string filename)
        {
            var basePath = AppContext.BaseDirectory;

            var builder = new ConfigurationBuilder()
                // .AddEnvironmentVariables()
                .SetBasePath(basePath)
                .AddJsonFile(filename + ".json", optional: false, reloadOnChange: true)
                .AddJsonFile(filename + ".local.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        private static T LoadSettings<T>(string filename) where T : new()
        {
            var configuration = BuildConfiguration(filename);

            var settings = configuration.Get<T>();

            return settings ?? new T();
        }
    }
}