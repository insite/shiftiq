using System;

using Shift.Common;

namespace InSite.Application.Banks.Read
{
    [Serializable]
    public class QBankFormFilter : Filter
    {
        public Guid? OrganizationIdentifier
        {
            get => OrganizationIdentifiers != null && OrganizationIdentifiers.Length == 1 ? OrganizationIdentifiers[0] : (Guid?)null;
            set => OrganizationIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] OrganizationIdentifiers { get; set; }
        public Guid? OccupationIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid? BankIdentifier { get; set; }
        public Guid? SpecIdentifier { get; set; }
        public Guid[] FormIdentifiers { get; set; }
        public Guid[] AttemptCandidateOrganizationIdentifiers { get; set; }
        public Guid? GradeItemIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }

        public string FormCode { get; set; }
        public string FormName { get; set; }
        public string FormTitle { get; set; }
        public string ExcludeFormStatus { get; set; }
        public string IncludeFormStatus { get; set; }
        public string Keyword { get; set; }
        public int? AssetNumber { get; set; }
        public string SpecType { get; set; }

        public QBankFormFilter Clone()
        {
            return (QBankFormFilter)MemberwiseClone();
        }
    }
}
