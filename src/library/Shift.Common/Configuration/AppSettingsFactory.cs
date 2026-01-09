using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using ioFile = System.IO.File;

namespace Shift.Common
{
    public static class AppSettingsFactory
    {
        private static Dictionary<string, bool> Cache = new Dictionary<string, bool>();

        public static AppSettings Create()
        {
            return LoadFromFile();
        }

        private static AppSettings LoadFromFile()
        {
            var locations = new[]
            {
                ".",
                Path.Combine(".", "bin"),
                Path.Combine(".", "bin", "debug"),
                Path.Combine("..", "..", "config")
            };

            var extensions = new[] { ".json", ".config" };

            var settingsFile = FindAppSettingsFile(locations, null, extensions);

            var settingsFolder = Path.GetDirectoryName(settingsFile);

            var settings = LoadAppSettingsFromFile(settingsFile, null);

            if (!settings.Environment.IsLocal())
                return settings;

            // PLEASE NOTE: Environment-specific settings and partition-specific settings are loaded ONLY when the 
            // file (appsettings.Target.json) is located in the same folder as the appsettings.json file loaded above.

            // Allow environment-specific settings to override the base settings in the Local environment only. This is
            // useful only for testing purposes. This behaviour is disabled in all other environments, which means the
            // CI/CD pipeline is responsible for ensuring environment-specific and partition-specific settings are 
            // merged into a static appsettings.json file during the deployment process.

            var environment = settings.Release.GetEnvironment().Name;

            settingsFile = Path.Combine(settingsFolder, $"appsettings.{environment}.json");

            settings = LoadAppSettingsFromFile(settingsFile, settings);

            // Allow partition-specific settings to override the base settings and the environment-specific settings 
            // only if this behaviour is explicitly enabled. This is sometimes useful (only) for testing purposes. 

            if (settings.Application.LoadPartitionSpecificSettings)
            {
                var partition = settings.Release.Partition;

                settingsFile = Path.Combine(settingsFolder, $"appsettings.{partition}.json");

                settings = LoadAppSettingsFromFile(settingsFile, settings);
            }

            return settings;
        }

        private static string FindAppSettingsFile(string[] locations, string specific, string[] extensions)
        {
            string file = null;

            for (var i = 0; i < locations.Length; i++)
            {
                foreach (var extension in extensions)
                {
                    var folder = locations[i];

                    file = CreatePhysicalPath(folder, specific, extension);

                    if (file != null)
                        break;

                    var baseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);

                    if (StringHelper.Equals(baseFolder, folder))
                        continue;

                    file = CreatePhysicalPath(baseFolder, specific, extension);

                    if (file != null)
                        break;
                }

                if (file != null)
                    break;
            }

            return file;
        }

        private static string CreatePhysicalPath(string folder, string specific, string extension)
        {
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

            var directory = Path.GetFullPath(Path.Combine(assemblyPath, folder));

            var file = "appsettings" + (specific == null ? "" : "." + specific) + extension;

            var path = Path.Combine(directory, file).ToLower();

            if (Cache.ContainsKey(path))
                return Cache[path] ? path : null;

            var exists = ioFile.Exists(path);

            Cache.Add(path, exists);

            if (exists)
                return path;

            return null;
        }

        private static AppSettings LoadAppSettingsFromFile(string jsonPath, AppSettings defaultSettings)
        {
            if (!ioFile.Exists(jsonPath))
                return defaultSettings;

            var encryptKey = Environment.GetEnvironmentVariable("SHIFTUI_APPSETTINGS_ENCRYPT_KEY");

            if (!string.IsNullOrEmpty(encryptKey))
            {
                using (var fileStream = ioFile.OpenRead(jsonPath))
                {
                    var settings = DecryptAppSettings(fileStream, encryptKey);

                    settings.ConfigurationProviders.Add(jsonPath);

                    return settings;
                }
            }
            else
            {
                var json = ioFile.ReadAllText(jsonPath);

                if (json.IsEmpty())
                    throw new Exception($"The application configuration setting file is empty: {jsonPath}");

                var settings = JsonConvert.DeserializeObject<AppSettings>(json, new AppSettingsConverter(defaultSettings));

                settings.ConfigurationProviders.Add(jsonPath);

                return settings;
            }
        }

        public static AppSettings DecryptAppSettings(Stream stream, string base64KeyIV)
        {
            var keyIV = Convert.FromBase64String(base64KeyIV);

            var key = new byte[32];
            Array.Copy(keyIV, key, 32);

            var iv = new byte[16];
            Array.Copy(keyIV, 32, iv, 0, 16);

            string json;

            using (var outputStream = new MemoryStream())
            {
                CryptoHelper.DecryptStreamWithoutSalt(iv, key, stream, outputStream);

                var jsonBytes = outputStream.GetBuffer();
                json = Encoding.UTF8.GetString(jsonBytes);
            }

            return JsonConvert.DeserializeObject<AppSettings>(json);
        }

        private class AppSettingsConverter : CustomCreationConverter<AppSettings>
        {
            private readonly AppSettings _defaultSettings;

            public AppSettingsConverter(AppSettings defaultSettings)
            {
                _defaultSettings = defaultSettings;
            }

            public override AppSettings Create(Type objectType)
            {
                return _defaultSettings ?? new AppSettings();
            }
        }
    }
}
