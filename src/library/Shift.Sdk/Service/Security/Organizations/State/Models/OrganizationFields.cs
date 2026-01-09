using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class OrganizationFields
    {
        public List<OrganizationField> User { get; set; } = new List<OrganizationField>();
        public List<OrganizationField> ClassRegistration { get; set; } = new List<OrganizationField>();
        public List<OrganizationField> LearnerDashboard { get; set; } = new List<OrganizationField>();
        public List<OrganizationField> InvoiceBillingAddress { get; set; } = new List<OrganizationField>();

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

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (User == null)
                User = new List<OrganizationField>();

            if (ClassRegistration == null)
                ClassRegistration = new List<OrganizationField>();

            if (LearnerDashboard == null)
                LearnerDashboard = new List<OrganizationField>();

            if (InvoiceBillingAddress == null)
                InvoiceBillingAddress = new List<OrganizationField>();
        }
    }
}