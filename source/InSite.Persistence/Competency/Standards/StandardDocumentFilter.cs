using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardDocumentFilter : Filter
    {
        [Serializable]
        public class PrivacyScopeInfo
        {
            public string Name { get; }
            public Guid User { get; }

            public PrivacyScopeInfo(string name, Guid user)
            {
                Name = name;
                User = user;
            }
        }

        public Guid? OrganizationIdentifier { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? DepartmentGroupIdentifier { get; set; }

        public string StandardType => Shift.Constant.StandardType.Document;
        public string DocumentType { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string Keyword { get; set; }
        public DateTimeRange Posted { get; set; }
        public bool? IsTemplate { get; set; }
        public bool? IsPortal { get; set; }
        public PrivacyScopeInfo PrivacyScope { get; set; }
        public Guid[] StandardIdentifiers { get; set; }

        public StandardDocumentFilter Clone()
        {
            return (StandardDocumentFilter)MemberwiseClone();
        }

        public StandardDocumentFilter()
        {
            Posted = new DateTimeRange();
        }
    }
}
