using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Validators.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QJournalSetupFilter>
    {
        public override QJournalSetupFilter Filter
        {
            get
            {
                var filter = new QJournalSetupFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    JournalSetupName = JournalSetupName.Text,
                    JournalSetupCreatedSince = JournalSetupCreatedSince.Value,
                    JournalSetupCreatedBefore = JournalSetupCreatedBefore.Value,
                    EventTitle = EventTitle.Text,
                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    AchievementIdentifier = AchievementIdentifier.Value,
                    ValidatorUserIdentifier = User.UserIdentifier
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                JournalSetupName.Text = value.JournalSetupName;
                JournalSetupCreatedSince.Value = value.JournalSetupCreatedSince;
                JournalSetupCreatedBefore.Value = value.JournalSetupCreatedBefore;
                EventTitle.Text = value.EventTitle;
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                AchievementIdentifier.Value = value.AchievementIdentifier;
            }
        }

        public override void Clear()
        {
            JournalSetupName.Text = null;
            JournalSetupCreatedSince.Value = null;
            JournalSetupCreatedBefore.Value = null;
            EventTitle.Text = null;
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            AchievementIdentifier.Value = null;
        }
    }
}