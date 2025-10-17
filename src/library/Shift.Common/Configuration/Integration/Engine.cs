namespace Shift.Common
{
    public class EngineSettings
    {
        public EngineApiSettings Api { get; set; }

        public ReleaseSettings Release { get; set; }

        public TelemetrySettings Telemetry { get; set; }

        public DatabaseSettings Database { get; set; }

        public SecuritySettings Security { get; set; }

        public IntegrationSettings Integration { get; set; }
    }

    public class EngineApiSettings
    {
        public ApiSettings Google { get; set; }
        public ApiSettings Premailer { get; set; }
        public ApiSettings Scorm { get; set; }
        public ApiSettings Scoop { get; set; }
        public ApiSettings ImageMagick { get; set; }
    }

    public class ShiftSettings
    {
        public ShiftSettingsApi Api { get; set; }
        public string ConfigurationProviders { get; set; }
    }

    public class ShiftSettingsApi
    {
        public ShiftSettingsApiVersions Hosting { get; set; }
        public string[] Origins { get; set; }
        public TelemetrySettings Telemetry { get; set; }
    }

    public class ShiftSettingsApiVersions
    {
        public ApiSettings V1 { get; set; }
        public ApiSettings V2 { get; set; }
    }
}