using System;

namespace InSite.Application.Standards.Read
{
    public class StandardRelationshipInfo
    {
        public Guid? ParentStandardIdentifier { get; set; }
        public Guid ChildStandardIdentifier { get; set; }
        public bool? IsHierarchy { get; set; }
        public bool? IsContainment { get; set; }
        public bool? IsConnection { get; set; }
        public int Depth { get; set; }
    }
}
