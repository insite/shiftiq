namespace Engine.Api.Internal
{
    public class SentrySettings
    {
        public required string Debug { get; set; }
        public required string Dsn { get; set; }
        public required string Log { get; set; }
        public required string LogFile { get; set; }
        public required double? SampleRate { get; set; }
        public required string Status { get; set; }

        public bool Enabled => Status == "Enabled";
        public bool DebugEnabled => Debug == "Enabled";
        public bool LogEnabled => Log == "Enabled";
    }
}