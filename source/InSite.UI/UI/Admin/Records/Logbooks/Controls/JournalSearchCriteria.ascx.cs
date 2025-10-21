using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Records.Logbooks.Controls
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

        public override void Clear()
        {
            JournalSetupIdentifier.Value = null;
            UserIdentifier.Value = null;
        }
    }
}