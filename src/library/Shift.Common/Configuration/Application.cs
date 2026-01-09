using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shift.Common
{
    public class Application
    {
        public string AbsoluteUrl { get; set; }
        public string EmailDomain { get; set; }
        public string EmailOutbox { get; set; }
        public int ProcessorTimeoutMinutes { get; set; }
        public string ResourceBundle { get; set; }
        public string ResourceLink { get; set; }

        public bool EmailOutboxDisabled => string.Compare(EmailOutbox, "Disabled", true) == 0;
        public bool EmailOutboxFiltered => string.Compare(EmailOutbox, "Filtered", true) == 0;
        public bool LoadPartitionSpecificSettings { get; set; }
        public bool ResourceBundleEnabled => ResourceBundle != "Disabled";
        public bool ResourceLinkDebug => ResourceLink == "Debug";
        public bool UseStrictModeForEmailEnabled { get; set; }

        public string AutoTranslate { get; set; }
        public bool AutoTranslateEnabled => string.Compare(AutoTranslate, "Enabled", true) == 0;

        public string DataPath { get; set; }
        public string ToolPath { get; set; }
        public string StylePath { get; set; }
        public string DefaultAvatarImageUrl { get; set; }
        public string SecurityConfigurationPath { get; set; }

        public string FFmpegFolderPath => Path.Combine(ToolPath, "ffmpeg");
        public string PandocExePath => Path.Combine(ToolPath, "pandoc", "pandoc.exe");
        public string WebKitHtmlToPdfExePath => Path.Combine(ToolPath, "wkhtmltopdf.exe");

        public string EncryptionKey { get; set; }
        public Organizations Organizations { get; set; }
        public string HelpUrl { get; set; }
        public int TempFileExpirationInMinutes { get; set; }
        public bool AntiForgeryTokenValidationEnabled { get; set; }

        public string AlertsToForceSend { get; set; }

        public List<string> AlertsToForceSendList =>
            string.IsNullOrWhiteSpace(AlertsToForceSend)
                ? new List<string>()
                : AlertsToForceSend
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .ToList();
    }
}