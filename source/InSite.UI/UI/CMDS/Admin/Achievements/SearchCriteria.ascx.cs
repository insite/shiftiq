using System;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant.CMDS;

namespace InSite.Cmds.Controls.Training.Achievements
{
    public partial class AchievementSearchCriteria : SearchCriteriaController<VCmdsAchievementFilter>
    {
        public override VCmdsAchievementFilter Filter
        {
            get
            {
                var filter = new VCmdsAchievementFilter
                {
                    Title = Title.Text,
                    AchievementType = SubType.Value,
                    AchievementVisibility = AccountScope.Value,
                    GlobalOrCompanySpecific = true,
                    AchievementOrganizationIdentifier = !Company.Enabled || CompanyPanel.Visible ? Company.Value : null,
                    CategoryIdentifier = CategoryPanel.Visible ? Category.ValueAsGuid : null,
                    IsTimeSensitive = IsTimeSensitive.ValueAsBoolean,
                    Description = Description.Text
                };

                return filter;
            }
            set
            {
                Title.Text = value.Title;
                SubType.Value = value.AchievementType;
                AccountScope.Value = value.AchievementVisibility;
                Company.Value = Company.Enabled ? value.AchievementOrganizationIdentifier : CurrentIdentityFactory.ActiveOrganizationIdentifier;
                IsTimeSensitive.ValueAsBoolean = value.IsTimeSensitive;

                InitVisibility();

                Category.ValueAsGuid = value.CategoryIdentifier;
                Description.Text = value.Description;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AccountScope.AutoPostBack = true;
            AccountScope.ValueChanged += Visibility_ValueChanged;

            Company.AutoPostBack = true;
            Company.ValueChanged += Company_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                InitVisibility();
        }

        private void Visibility_ValueChanged(object sender, EventArgs e)
        {
            InitVisibility();
        }

        private void Company_ValueChanged(object o, EventArgs e)
        {
            LoadCategories();
        }

        public override void Clear()
        {
            Title.Text = null;
            SubType.ClearSelection();
            AccountScope.ClearSelection();
            Company.Value = AccountScope.IsGlobalItemVisible ? (Guid?)null : CurrentIdentityFactory.ActiveOrganizationIdentifier;
            IsTimeSensitive.ClearSelection();
            Description.Text = null;

            InitVisibility();
        }

        private void InitVisibility()
        {
            switch (AccountScope.Value)
            {
                case AccountScopes.Organization:
                    CompanyPanel.Visible = true;
                    CategoryPanel.Visible = true;
                    break;
                default:
                    CompanyPanel.Visible = false;
                    CategoryPanel.Visible = false;
                    break;
            }

            LoadCategories();
        }

        private void LoadCategories()
        {
            if (AccountScope.Value == AccountScopes.Enterprise)
                return;

            var oldCategoryIdentifier = Category.Value;

            Category.ListFilter.OrganizationIdentifier = Company.Value ?? Guid.Empty;
            Category.RefreshData();

            Category.Value = oldCategoryIdentifier;
        }
    }
}