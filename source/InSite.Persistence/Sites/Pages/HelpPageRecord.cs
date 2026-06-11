using System;

namespace InSite.Persistence
{
    public class HelpPageRecord
    {
        public Guid WebSiteIdentifier { get; set; }
        public Guid WebPageIdentifier { get; set; }
        public string PathUrl { get; set; }
        public string WebPageTitle { get; set; }
        public string WebPageType { get; set; }
        public string ContentLabel { get; set; }
        public string ContentSnip { get; set; }
        public string ContentText { get; set; }
        public string ContentHtml { get; set; }
    }
}
