using System.Collections.Generic;

namespace InSite.Persistence.Content
{
    public class PortalPageList
    {
        public string Body { get; set; }
        public string Icon { get; set; }
        public string LastUpdated { get; set; }
        public string Summary { get; set; }
        public string SupportUrl { get; set; }
        public string Title { get; set; }

        public List<LaunchCard> Items { get; set; }
    }
}