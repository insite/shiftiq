using System;

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

    public class PartitionRegistrationSettings
    {
        public bool Enabled { get; set; }
        public int TimeoutMinutes { get; set; }
    }

    public class EngineApiSettings
    {
        // Single root for all Engine (Hub) API libraries and third-party integrations.

        // The per-integration base URLs below are derived by appending the integration's route
        // segment, so configuration needs only one key.

        public string BaseUrl { get; set; }

        public PartitionRegistrationSettings PartitionRegistration { get; set; }

        // Shared service key sent as X-Api-Key on Engine (Hub) calls that run server-side.

        public string ApiKey { get; set; }

        public ApiSettings Google => Segment("google/");
        public ApiSettings Premailer => Segment("premailer/");
        public ApiSettings ImageMagick => Segment("imagemagick/");

        private ApiSettings _scorm;
        public ApiSettings Scorm
        {
            get
            {
                // ScormCloud carries a configured CallbackPath; fill in the derived BaseUrl while
                // preserving whatever was bound from configuration.

                var settings = _scorm ?? new ApiSettings();

                if (string.IsNullOrEmpty(settings.BaseUrl))
                    settings.BaseUrl = Segment("scorm/")?.BaseUrl;

                return settings;
            }
            set => _scorm = value;
        }

        public ScoopSettings Scoop { get; set; }

        private ApiSettings Segment(string segment)
        {
            if (string.IsNullOrEmpty(BaseUrl))
                return null;

            var root = BaseUrl.EndsWith("/") ? BaseUrl : BaseUrl + "/";

            return new ApiSettings { BaseUrl = root + segment };
        }
    }

    public class ShiftSettings
    {
        public ShiftSettingsApi Api { get; set; }
        public ShiftSettingsMailgun Mailgun { get; set; }
        public string ConfigurationProviders { get; set; }
    }

    public class ShiftSettingsApi
    {
        public ApiSettings Hosting { get; set; }
        public string[] Origins { get; set; }
        public TelemetrySettings Telemetry { get; set; }
    }

    public class ShiftSettingsMailgun
    {
        public TelemetrySettings Telemetry { get; set; }
    }

    public class PartitionSettings : IPartitionModel
    {
        public int Number { get; set; }

        public string Name { get; set; }
        public string Brand { get; set; }
        public string Style { get; set; }
        public string Domain { get; set; }
        public string Email { get; set; }
        public string Slug { get; set; }
        public string HelpUrl { get; set; }
        public string LogoUrl { get; set; }

        public Guid Identifier { get; set; }

        public string WhitelistDomains { get; set; }
        public string WhitelistEmails { get; set; }

        public bool IsE01() => Number == 1;
        public bool IsE02() => Number == 2;
        public bool IsE03() => Number == 3;
        public bool IsE04() => Number == 4;
        public bool IsE07() => Number == 7;

        private string _tenant;
        public string Tenant
        {
            get
            {
                if (_tenant == null)
                    _tenant = "E" + StringHelper.PadLeft(Number.ToString(), "0", 2);

                return _tenant;
            }
        }
    }
}
