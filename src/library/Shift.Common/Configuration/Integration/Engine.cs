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

    public class EngineApiSettings
    {
        public ApiSettings Google { get; set; }
        public ApiSettings Premailer { get; set; }
        public ApiSettings Scorm { get; set; }
        public ScoopSettings Scoop { get; set; }
        public ApiSettings ImageMagick { get; set; }
    }

    public class ShiftSettings
    {
        public ShiftSettingsApi Api { get; set; }
        public string ConfigurationProviders { get; set; }
    }

    public class ShiftSettingsApi
    {
        public ApiSettings Hosting { get; set; }
        public string[] Origins { get; set; }
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