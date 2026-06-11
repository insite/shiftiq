using System;

using Shift.Common;

namespace InSite.Application.Banks.Read
{
    [Serializable]
    public class QBankFilter : Filter
    {
        public Guid? OrganizationIdentifier
        {
            get => OrganizationIdentifiers != null && OrganizationIdentifiers.Length == 1 ? OrganizationIdentifiers[0] : (Guid?)null;
            set => OrganizationIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] OrganizationIdentifiers { get; set; }
        public Guid? OccupationIdentifier { get; set; }
        public Guid[] FrameworkIdentifiers { get; set; }
        public Guid[] AttemptCandidateOrganizationIdentifiers { get; set; }
        public Guid? DepartmentIdentifier { get; set; }

        public string BankName { get; set; }
        public string BankStatus { get; set; }
        public string BankTitle { get; set; }
        public string BankType { get; set; }
        public string Keyword { get; set; }
        public int? AssetNumber { get; set; }
        public bool? BankEnable { get; set; }

        public QBankFilter Clone()
        {
            return (QBankFilter)MemberwiseClone();
        }
    }
}
