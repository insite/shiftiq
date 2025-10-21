using System;

namespace InSite.Persistence
{
    public class TLtiLink
    {
        public Guid LinkIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public String ResourceCode { get; set; }
        public String ResourceName { get; set; }
        public String ResourceParameters { get; set; }
        public String ResourceSummary { get; set; }
        public String ResourceTitle { get; set; }
        public String ToolConsumerKey { get; set; }
        public String ToolConsumerSecret { get; set; }
        public String ToolProviderName { get; set; }
        public String ToolProviderType { get; set; }
        public String ToolProviderUrl { get; set; }
        public Int32 AssetNumber { get; set; }
    }
}
