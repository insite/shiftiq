namespace Shift.Common
{
    public class ApiSettings
    {
        // BaseUrl is preferred for configuration keys. Use Host only when address is split into Scheme, Host, Port).
        public string BaseUrl { get; set; }

        // Avoid Url and Uri in configuration keys for relative values.
        public string CallbackPath { get; set; }
    }
}