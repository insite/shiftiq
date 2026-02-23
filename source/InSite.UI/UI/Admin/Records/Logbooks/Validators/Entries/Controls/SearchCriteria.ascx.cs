using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Records.Validators.Entries.Controls
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
                    ValidatorUserIdentifier = User.UserIdentifier,

                    JournalSetupIdentifier = JournalSetupIdentifier.Value,
                    UserDepratmentIdentifier = DepartmentIdentifier.Value,
                    UserIdentifier = UserIdentifier.Value,
                    CreatedSince = CreatedSince.Value,
                    CreatedBefore = CreatedBefore.Value,
                    IsValidated = ValidationStatus.ValueAsBoolean,

                    TrainingTypeExact = TrainingType.Value,
                    EmployerContains = Employer.Text,
                    SupervisorContains = Supervisor.Text,
                    StartDateExact = StartDate.Value,
                    EndDateExact = EndDate.Value,
                    HoursExact = Hours.ValueAsInt,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                JournalSetupIdentifier.Value = value.JournalSetupIdentifier;
                DepartmentIdentifier.Value = value.UserDepratmentIdentifier;
                UserIdentifier.Value = value.UserIdentifier;
                CreatedSince.Value = value.CreatedSince;
                CreatedBefore.Value = value.CreatedBefore;
                ValidationStatus.ValueAsBoolean = value.IsValidated;

                TrainingType.Value = value.TrainingTypeExact;
                Employer.Text = value.EmployerContains;
                Supervisor.Text = value.SupervisorContains;
                StartDate.Value = value.StartDateExact;
                EndDate.Value = value.EndDateExact;
                Hours.ValueAsInt = value.HoursExact;
            }
        }

        public override void Clear()
        {
            JournalSetupIdentifier.Value = null;
            DepartmentIdentifier.Value = null;

            OnDepartmentChanged();

            CreatedSince.Value = null;
            CreatedBefore.Value = null;
            ValidationStatus.ClearSelection();

            TrainingType.Value = null;
            Employer.Text = null;
            Supervisor.Text = null;
            StartDate.Value = null;
            EndDate.Value = null;
            Hours.ValueAsInt = null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            JournalSetupIdentifier.Filter.ValidatorUserIdentifier = User.UserIdentifier;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.OrganizationIdentifier = Organization.Identifier;
            DepartmentIdentifier.ValueChanged += (s, a) => OnDepartmentChanged();
        }

        private void OnDepartmentChanged()
        {
            UserIdentifier.Filter.GroupDepartmentIdentifiers = DepartmentIdentifier.HasValue
                ? new[] { DepartmentIdentifier.Value.Value }
                : null;
            UserIdentifier.Value = null;
        }
    }
}