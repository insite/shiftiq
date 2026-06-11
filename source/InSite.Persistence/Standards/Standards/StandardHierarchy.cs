using System;

namespace InSite.Persistence
{
    public class StandardHierarchy
    {
        public Guid? ParentStandardIdentifier { get; set; }
        public Guid? RootStandardIdentifier { get; set; }
        public Guid? StandardIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string Name { get; set; }
        public string PathAsset { get; set; }
        public string PathCode { get; set; }
        public string PathKey { get; set; }
        public string PathName { get; set; }
        public string PathSequence { get; set; }
        public string SourceDescriptor { get; set; }
        public string StandardLabel { get; set; }
        public string StandardType { get; set; }
        public string Title { get; set; }

        public bool? IsHidden { get; set; }
        public bool? IsPractical { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsTheory { get; set; }

        public int? AssetNumber { get; set; }
        public int? Depth { get; set; }
        public int? Sequence { get; set; }
    }
}
