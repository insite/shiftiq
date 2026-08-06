using System;
using System.Web.UI;

using InSite.UI.Layout.Admin;

using Shift.Common.Events;
using Shift.Constant;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class ProgramAchievementsCriteria : AdminBaseControl
    {
        public event AlertHandler Alert;

        public string ValidationGroup
        {
            get => AchievementSelectorValidator.ValidationGroup;
            set => AchievementSelectorValidator.ValidationGroup = value;
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

            AchievementSelectorValidator.ServerValidate += (s, a) => a.IsValid = AchievementSelector.HasValue();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
                return;

            OnGroupTypeChanged();
            LoadAchievements();
        }

        private void OnGroupTypeChanged()
        {
            FindGroup.Filter.GroupType = GroupType.Value;
            FindGroup.Value = null;
        }

        private void LoadAchievements()
        {
            var hasAchievements = AchievementSelector.LoadDataByOrganization(Organization.Identifier, null, null);

            AchievementSelector.Visible = hasAchievements;

            if (!hasAchievements)
                Alert?.Invoke(this, new AlertArgs(AlertType.Error, "There are no training achievements for this organization."));
        }
    }
}
