using System;

namespace InSite.Persistence
{
    public class StandardContainmentSummary
    {
        public int ChildAssetNumber { get; set; }
        public string ChildIcon { get; set; }
        public string ChildName { get; set; }
        public int ChildSequence { get; set; }
        public Guid ChildStandardIdentifier { get; set; }
        public string ChildStandardType { get; set; }
        public Guid ChildOrganizationIdentifier { get; set; }
        public string ChildTitle { get; set; }
        public int ParentAssetNumber { get; set; }
        public string ParentIcon { get; set; }
        public bool ParentIsPrimaryContainer { get; set; }
        public string ParentName { get; set; }
        public int ParentSequence { get; set; }
        public Guid ParentStandardIdentifier { get; set; }
        public string ParentStandardType { get; set; }
        public Guid ParentOrganizationIdentifier { get; set; }
        public string ParentTitle { get; set; }
    }
}
