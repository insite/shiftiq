using System.Collections.Generic;
using System.IO;

namespace Shift.Common
{
    public class AppSettings
    {
        public ReleaseSettings Release { get; set; }
        public Application Application { get; set; }
        public SecuritySettings Security { get; set; }
        public DatabaseSettings Database { get; set; }
        public TimelineSettings Timeline { get; set; }
        public IntegrationSettings Integration { get; set; }
        public Variant Variant { get; set; }
        public Platform Platform { get; set; }
        public EngineSettings Engine { get; set; }
        public ShiftSettings Shift { get; set; }

        public List<string> ConfigurationProviders { get; set; } = new List<string>();

        public string DataFolderEnterprise => Path.Combine(Application.DataPath, Release.Partition, Release.Environment);

        public string DataFolderShare => Path.Combine(Application.DataPath, "E00", Release.Environment);

        public EnvironmentModel Environment => Release.GetEnvironment();
    }
}