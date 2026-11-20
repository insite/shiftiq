namespace Shift.Common
{
    public class ApiSettings
    {
        // BaseUrl is preferred for configuration keys. Use Host only when address is split into Scheme, Host, Port).
        public string BaseUrl { get; set; }

        // Avoid Url and Uri in configuration keys for relative values.
        public string CallbackPath { get; set; }

        // Sent as X-Hub-Api-Key when set. EngineApiSettings fills it from Engine:Api:ApiKey for the
        // Google, Premailer and ImageMagick segments.
        public string ApiKey { get; set; }
    }
}
