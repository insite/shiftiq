using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Training.Achievements
{
    public partial class FieldAchievementSearchCriteria : SearchCriteriaController<VCmdsAchievementFilter>
    {
        public override VCmdsAchievementFilter Filter
        {
            get
            {
                var filter = new VCmdsAchievementFilter
                {
                    Title = Title.Text,
                    AchievementType = SubType.Value,
                    AchievementOrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                    CategoryIdentifier = CategoryPanel.Visible ? Category.ValueAsGuid : null,
                    IsTimeSensitive = IsTimeSensitive.ValueAsBoolean
                };

                return filter;
            }
            set
            {
                var filter = value;
                Title.Text = filter.Title;
                SubType.Value = filter.AchievementType;
                IsTimeSensitive.ValueAsBoolean = filter.IsTimeSensitive;

                InitVisibility();

                Category.ValueAsGuid = filter.CategoryIdentifier;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                InitVisibility();
        }

        private void Visibility_ValueChanged(Object sender, EventArgs e)
        {
            InitVisibility();
        }

        public override void Clear()
        {
            Title.Text = null;
            SubType.ClearSelection();
            Category.ClearSelection();
            IsTimeSensitive.ClearSelection();

            InitVisibility();
        }

        private void InitVisibility()
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            var oldCategoryIdentifier = Category.ValueAsGuid;

            Category.ListFilter.OrganizationIdentifier = OrganizationSearch.Select(CurrentIdentityFactory.ActiveOrganizationIdentifier).OrganizationIdentifier;
            Category.RefreshData();

            Category.ValueAsGuid = oldCategoryIdentifier;
        }
    }
}