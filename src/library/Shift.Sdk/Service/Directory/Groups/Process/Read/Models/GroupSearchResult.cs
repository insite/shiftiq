using System;
using System.Linq;

namespace InSite.Application.Contacts.Read
{
    public class GroupSearchResult
    {
        public class ParentInfo
        {
            public Guid? GroupIdentifier { get; set; }
            public string GroupName { get; set; }
        }

        public Guid GroupIdentifier { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string GroupType { get; set; }
        public string GroupLabel { get; set; }
        public string GroupCode { get; set; }
        public string GroupCategory { get; set; }
        public string GroupStatus { get; set; }
        public string GroupOffice { get; set; }
        public string GroupPhone { get; set; }
        public string GroupRegion { get; set; }
        public string NumberOfEmployees { get; set; }
        public int? GroupCapacity { get; set; }
        public int GroupSize { get; set; }
        public DateTimeOffset? GroupExpiry { get; set; }
        public int ChildCount { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public string SurveyFormName { get; set; }
        public Guid? MembershipProductIdentifier { get; set; }
        public int MembershipStatusSize { get; set; }
        public string MembershipProductName { get; set; }
        public ParentInfo HierarchyParent { get; set; }
        public IOrderedEnumerable<ParentInfo> FunctionalParents { get; set; }
        public QGroupAddress ShippingAddress { get; set; }
        public QGroupAddress BillingAddress { get; set; }
        public QGroupAddress PhysicalAddress { get; set; }
    }
}
