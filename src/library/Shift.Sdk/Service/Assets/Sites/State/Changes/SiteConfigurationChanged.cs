using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Sites
{
    public class SiteConfigurationChanged : Change
    {
        public string Configruation { get; set; }
        public SiteConfigurationChanged(string configuration)
        {
            Configruation = configuration;
        }
    }
}
