using System;
using System.Web.UI;

using InSite.UI.Layout.Admin;

using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class ProgramEnrollmentsCriteria : AdminBaseControl
    {
        public event AlertHandler Alert;

        public string ValidationGroup
        {
            get => FindProgramValidator.ValidationGroup;
            set => FindProgramValidator.ValidationGroup = value;
        }

        public bool HasPrograms
        {
            get => (bool)(ViewState[nameof(HasPrograms)] ?? false);
            set => ViewState[nameof(HasPrograms)] = value;
        }

        public string GroupTypeValue => GroupType.Value;

        public Guid[] GroupValues => FindGroup.Values;

        public Guid[] ProgramValues => FindProgram.Values;

        public Guid[] LearnerValues => FindLearner.Values;

        public string CredentialStatusValue => CredentialStatus.Value;

        public Guid[] SelectedAchievements => AchievementSelector.GetSelectedAchievements();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupType.AutoPostBack = true;
            GroupType.ValueChanged += (s, a) => OnGroupTypeChanged();

            FindProgramValidator.ServerValidate += (s, a) => a.IsValid = HasPrograms && FindProgram.HasValue;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            OnGroupTypeChanged();
            LoadAchievements();

            HasPrograms = FindProgram.HasAnyItem();

            if (!HasPrograms)
            {
                FindProgram.EmptyMessage = string.Empty;
                Alert?.Invoke(this, new AlertArgs(AlertType.Error, "There are no programs for this organization."));
            }
        }

        private void OnGroupTypeChanged()
        {
            FindGroup.Filter.GroupType = GroupType.Value;
            FindGroup.Value = null;
        }

        private void LoadAchievements()
        {
            var hasAchievements = AchievementSelector.LoadDataByOrganization(Organization.Identifier, new[] { "Certification" }, null);

            AchievementSelector.Visible = hasAchievements;
        }
    }
}
