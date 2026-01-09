using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Logbooks.Entries.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QExperienceFilter>
    {
        public override QExperienceFilter Filter
        {
            get
            {
                var filter = new QExperienceFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    JournalSetupIdentifier = JournalSetupIdentifier.Value,
                    UserIdentifier = UserIdentifier.Value,
                    TrainingType = TrainingTypeComboBox.Value,
                    CreatedSince = CreatedSince.Value,
                    CreatedBefore = CreatedBefore.Value,
                    IsValidated = ValidationStatus.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                JournalSetupIdentifier.Value = value?.JournalSetupIdentifier;
                UserIdentifier.Value = value?.UserIdentifier;
                CreatedSince.Value = value?.CreatedSince;
                CreatedBefore.Value = value?.CreatedBefore;
                ValidationStatus.ValueAsBoolean = value.IsValidated;
                TrainingTypeComboBox.Value = value.TrainingType;
            }
        }

        public override void Clear()
        {
            JournalSetupIdentifier.Value = null;
            UserIdentifier.Value = null;
            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            TrainingTypeComboBox.Value = null;
            ValidationStatus.ClearSelection();
        }
    }
}