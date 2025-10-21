using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Logbooks.Competencies.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QExperienceCompetencyFilter>
    {
        public override QExperienceCompetencyFilter Filter
        {
            get
            {
                var filter = new QExperienceCompetencyFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    JournalSetupIdentifier = JournalSetupIdentifier.Value,
                    UserIdentifier = UserIdentifier.Value,

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
            }
        }

        public override void Clear()
        {
            JournalSetupIdentifier.Value = null;
            UserIdentifier.Value = null;
            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            ValidationStatus.ClearSelection();
        }
    }
}