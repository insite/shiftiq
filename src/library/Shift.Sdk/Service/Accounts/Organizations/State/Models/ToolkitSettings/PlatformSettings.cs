using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class PlatformSettings
    {
        public PlatformDocsSettings Docs { get; set; }

        public PlatformSettings()
        {
            Docs = new PlatformDocsSettings();
        }
    }
}
