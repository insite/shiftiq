using System;

using Shift.Common;

namespace InSite.Application.Banks.Read
{
    [Serializable]
    public class QBankSpecificationFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? BankIdentifier { get; set; }
        public string SpecName { get; set; }
        public string SpecType { get; set; }
        public int? SpecAsset { get; set; }

        public QBankSpecificationFilter Clone()
        {
            return (QBankSpecificationFilter)MemberwiseClone();
        }
    }
}
