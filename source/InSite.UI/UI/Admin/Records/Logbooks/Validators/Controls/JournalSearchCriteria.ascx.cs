using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Controls
{
    public partial class JournalSearchCriteria : SearchCriteriaController<VJournalSetupUserFilter>
    {
        public override VJournalSetupUserFilter Filter
        {
            get
            {
                var filter = new VJournalSetupUserFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    JournalSetupIdentifier = JournalSetupIdentifier.Value,
                    UserIdentifier = UserIdentifier.Value,
                    Role = JournalSetupUserRole.Learner,
                    ValidatorUserIdentifier = User.UserIdentifier,
                    OrderBy = "JournalCreated"
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                JournalSetupIdentifier.Value = value?.JournalSetupIdentifier;
                UserIdentifier.Value = value?.UserIdentifier;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                JournalSetupIdentifier.Filter.ValidatorUserIdentifier = User.UserIdentifier;
        }

        public override void Clear()
        {
            JournalSetupIdentifier.Value = null;
            UserIdentifier.Value = null;
        }
    }
}