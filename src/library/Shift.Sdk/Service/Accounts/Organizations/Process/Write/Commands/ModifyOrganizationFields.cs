using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Organizations;

using Shift.Constant;

namespace InSite.Application.Organizations.Write
{
    public class ModifyOrganizationFields : Command, IHasRun
    {
        public OrganizationFields Fields { get; set; }

        public ModifyOrganizationFields(Guid organizationId, OrganizationFields fields)
        {
            AggregateIdentifier = organizationId;
            Fields = fields;
        }

        bool IHasRun.Run(OrganizationAggregate aggregate)
        {
            var state = aggregate.Data;
            if (state.AccountStatus != AccountStatus.Opened)
                return false;

            var isUserSame = OrganizationField.IsEqual(state.Fields.User, Fields.User);
            var isClassRegistrationSame = OrganizationField.IsEqual(state.Fields.ClassRegistration, Fields.ClassRegistration);
            var isLearnerDashboardSame = OrganizationField.IsEqual(state.Fields.LearnerDashboard, Fields.LearnerDashboard);
            var isInvoiceBillingAddressSame = OrganizationField.IsEqual(state.Fields.InvoiceBillingAddress, Fields.InvoiceBillingAddress);

            if (isUserSame && isClassRegistrationSame && isLearnerDashboardSame && isInvoiceBillingAddressSame)
                return true;

            if (isUserSame)
                Fields.User = null;

            if (isClassRegistrationSame)
                Fields.ClassRegistration = null;

            if (isLearnerDashboardSame)
                Fields.LearnerDashboard = null;

            if (isInvoiceBillingAddressSame)
                Fields.InvoiceBillingAddress = null;

            aggregate.Apply(new OrganizationFieldsModified(Fields));

            return true;
        }
    }
}
