namespace Shift.Test
{
    public static class AppSettingsHelper
    {
        public static T GetAllSettings<T>() where T : new()
        {
            var configuration = BuildConfiguration();

            var settings = configuration.Get<T>();

            return settings ?? new T();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var path = AppContext.BaseDirectory;

            var builder = new ConfigurationBuilder().SetBasePath(path);

            AddSettings(builder, path, "appsettings.json");

            AddSettings(builder, path, "appsettings.local.json");

            return builder.Build();
        }

        private static void AddSettings(IConfigurationBuilder builder, string path, string file)
        {
            if (TryAddFile(builder, path, file))
                return;

            if (TryAddFile(builder, Path.Combine(path, ".."), file))
                return;

            if (TryAddFile(builder, Path.Combine(path, "..", ".."), file))
                return;

            TryAddFile(builder, Path.Combine(path, "..", "..", ".."), file);
        }

        private static bool TryAddFile(IConfigurationBuilder builder, string folder, string file)
        {
            var path = Path.Combine(folder, file);

            if (!File.Exists(path))
                return false;

            builder = builder.AddJsonFile(path);

            return true;
        }
    }
}