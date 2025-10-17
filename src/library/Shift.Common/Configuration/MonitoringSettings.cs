namespace Shift.Common
{
    public class MonitoringSettings
    {
        public string BaseUrl { get; set; }
        public bool Debug { get; set; }
        public string Dsn { get; set; }
        public bool Enabled { get; set; }
        public string File { get; set; }
        public string Project { get; set; }
        public double? Rate { get; set; }

        public bool Disabled => !Enabled;
    }
}
