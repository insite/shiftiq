using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class OrganizationFields
    {
        public List<OrganizationField> User { get; set; }
        public List<OrganizationField> ClassRegistration { get; set; }
        public List<OrganizationField> LearnerDashboard { get; set; }
        public List<OrganizationField> InvoiceBillingAddress { get; set; }

        public OrganizationFields()
        {
            User = new List<OrganizationField>();
            ClassRegistration = new List<OrganizationField>();
            LearnerDashboard = new List<OrganizationField>();
            InvoiceBillingAddress = new List<OrganizationField>();
        }

        public bool IsVisible(string item, List<OrganizationField> list, bool @default = true)
        {
            var field = list.FirstOrDefault(f => StringHelper.Equals(f.FieldName, item));
            return field != null ? field.IsVisible : @default;
        }

        public OrganizationFields Clone()
        {
            return new OrganizationFields 
            {
                User = User.Select(x => x.Clone()).ToList(),
                ClassRegistration = ClassRegistration.Select(x => x.Clone()).ToList(),
                LearnerDashboard = LearnerDashboard.Select(x => x.Clone()).ToList(),
                InvoiceBillingAddress = InvoiceBillingAddress.Select(x => x.Clone()).ToList(),
            };
        }
    }
}