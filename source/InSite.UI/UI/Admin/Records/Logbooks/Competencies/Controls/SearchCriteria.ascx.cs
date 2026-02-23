using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

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
                    CompetencyStandardIdentifier = CompetencyIdentifier.Enabled && CompetencyIdentifier.HasValue
                        ? new[] { CompetencyIdentifier.Value.Value }
                        : null,

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

                OnJournalSetupChanged();

                CompetencyIdentifier.Value = CompetencyIdentifier.Enabled && (value?.CompetencyStandardIdentifier).IsNotEmpty()
                    ? value.CompetencyStandardIdentifier[0]
                    : (Guid?)null;

                CreatedSince.Value = value?.CreatedSince;
                CreatedBefore.Value = value?.CreatedBefore;
                ValidationStatus.ValueAsBoolean = value.IsValidated;
            }
        }

        public override void Clear()
        {
            JournalSetupIdentifier.Value = null;
            UserIdentifier.Value = null;

            OnJournalSetupChanged();

            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            ValidationStatus.ClearSelection();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            JournalSetupIdentifier.AutoPostBack = true;
            JournalSetupIdentifier.ValueChanged += (s, a) => OnJournalSetupChanged();
        }

        private void OnJournalSetupChanged()
        {
            CompetencyIdentifier.Value = null;
            CompetencyIdentifier.Enabled = false;
            CompetencyIdentifier.Filter.RootStandardIdentifier = Guid.Empty;
            CompetencyIdentifier.Filter.StandardTypes = new string[] { StandardType.Competency };

            if (!JournalSetupIdentifier.HasValue)
                return;

            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupIdentifier.Value.Value);
            if (journalSetup?.FrameworkStandardIdentifier == null)
                return;

            CompetencyIdentifier.Filter.RootStandardIdentifier = journalSetup.FrameworkStandardIdentifier.Value;
            CompetencyIdentifier.Enabled = true;
        }
    }
}