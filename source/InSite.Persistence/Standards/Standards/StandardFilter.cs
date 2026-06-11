using System;
using System.Collections.Generic;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardFilter : Filter
    {
        public DateTimeRange Modified { get; set; }
        public bool? HasChildren { get; set; }
        public bool? HasParent { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsPublished { get; set; }
        public int? Number { get; set; }
        public string StandardLabel { get; set; }
        public string StandardTier { get; set; }
        public Guid? ParentStandardIdentifier
        {
            get => ParentStandardIdentifiers != null && ParentStandardIdentifiers.Length == 1 ? ParentStandardIdentifiers[0] : (Guid?)null;
            set => ParentStandardIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] ParentStandardIdentifiers { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? DepartmentGroupIdentifier
        {
            get => DepartmentGroupIdentifiers != null && DepartmentGroupIdentifiers.Length == 1 ? DepartmentGroupIdentifiers[0] : (Guid?)null;
            set => DepartmentGroupIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] DepartmentGroupIdentifiers { get; set; }
        public Guid[] StandardIdentifiers { get; set; }
        public bool? HasCode { get; set; }
        public string Code { get; set; }
        public string ParentTitle { get; set; }
        public string ContentName { get; set; }
        public string SelectorText { get; set; }
        public string Title { get; set; }
        public string Keyword { get; set; }
        public StandardTypeEnum Scope { get; set; } = StandardTypeEnum.None;
        public string[] Tags { get; set; }
        public Guid? ValidationUserIdentifier { get; set; }
        public Guid[] Inclusions { get; set; }
        public string[] DocumentType { get; set; }
        public string[] StandardTypes { get; set; }
        public Guid? RootStandardIdentifier { get; set; }
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? PortalUserIdentifier { get; set; }
        public bool? IsPortal { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? CategoryItemIdentifier { get; set; }

        #region Exclusions

        public ExclusionSet Exclusions { get; private set; }

        [Serializable]
        public class ExclusionSet
        {
            public bool? IsHidden { get; set; }
            public bool? IsPublished { get; set; }

            public List<Guid> StandardIdentifier { get; set; }
            public List<string> StandardType { get; set; }

            public ExclusionSet()
            {
                StandardIdentifier = new List<Guid>();
                StandardType = new List<string>();
            }
        }

        #endregion

        public StandardFilter()
        {
            StandardTypes = null;

            Modified = new DateTimeRange();

            Exclusions = new ExclusionSet();
        }

        public StandardFilter Clone()
        {
            return (StandardFilter)MemberwiseClone();
        }
    }
}