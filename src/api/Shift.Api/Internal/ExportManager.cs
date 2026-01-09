using Shift.Common;

namespace Shift.Api.Internal
{
    public class ExportManager
    {
        private readonly DatabaseSettings _databaseSettings;

        private readonly string _component;

        private readonly string _entity;

        public ExportManager(DatabaseSettings databaseSettings, string component, string entity)
        {
            _databaseSettings = databaseSettings;
            _component = component;
            _entity = entity;
        }

        public ExportCompleted Find(string key)
        {
            var completed = new ExportCompleted();

            completed.ExportKey = key;

            completed.ExportName = _entity;

            var folder = GetFolderPath();

            var jsonFile = Path.Combine(folder, $"{key}.json");

            if (File.Exists(jsonFile))
            {
                completed.PhysicalFile = jsonFile;
                completed.ExportFormat = "json";
                return completed;
            }

            var csvFile = Path.Combine(folder, $"{key}.csv");

            if (File.Exists(csvFile))
            {
                completed.PhysicalFile = csvFile;
                completed.ExportFormat = "csv";
                return completed;
            }

            return completed;
        }

        public StartExport Start(string format)
        {
            var start = new StartExport
            {
                ExportKey = Guid.NewGuid().ToString(),

                ExportFormat = format ?? "json",

                Expiry = CalculateNextExpiry()
            };

            var folder = GetFolderPath();

            start.PhysicalFile = Path.Combine(folder, $"{start.ExportKey}.{start.ExportFormat}");

            return start;
        }

        public ExportStarted Started(StartExport start, string downloadUrl)
        {
            var started = new ExportStarted
            {
                ExportKey = start.ExportKey,

                ExportFormat = start.ExportFormat,

                DownloadUrl = downloadUrl.Replace("{key}", start.ExportKey),

                Expiry = start.Expiry
            };

            return started;
        }

        /// <summary>
        /// Returns the first 15-minute interval after 15 minutes has elapsed, for a maximum lifetime of 30 minutes.
        /// </summary>
        private DateTimeOffset CalculateNextExpiry()
        {
            var expiry = DateTimeOffset.Now.AddMinutes(15);

            var minutesToAdd = 15 - (expiry.Minute % 15);

            if (minutesToAdd == 15 && expiry.Second == 0 && expiry.Millisecond == 0)
                return expiry; // already on a 15-minute boundary

            return new DateTime(expiry.Year, expiry.Month, expiry.Day, expiry.Hour, expiry.Minute, 0)
                .AddMinutes(minutesToAdd);
        }

        private string GetFolderPath()
        {
            var folder = Path.Combine(_databaseSettings.FileStorage, "exports", _component, _entity);

            Directory.CreateDirectory(folder);

            return folder;
        }
    }
}
