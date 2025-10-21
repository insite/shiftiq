using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Training.Achievements
{
    public partial class AchievementHierarchy : UserControl
    {
        public bool Enabled
        {
            get { return ViewState[nameof(Enabled)] == null ? false : (bool)ViewState[nameof(Enabled)]; }
            set { ViewState[nameof(Enabled)] = value; }
        }

        public string AchievementType
        {
            get { return ViewState[nameof(AchievementType)] == null ? null : (string)ViewState[nameof(AchievementType)]; }
            private set { ViewState[nameof(AchievementType)] = value; }
        }

        public Guid OrganizationIdentifier
        {
            get => (ViewState[nameof(OrganizationIdentifier)] as Guid?)
                ?? CurrentSessionState.Identity.Organization.Identifier;
            private set
            {
                ViewState[nameof(OrganizationIdentifier)] = value;
            }
        }

        public void SetDefaultValues()
        {
            OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            InitCompany();
        }

        public void SetAchievementType(string achievementType)
        {
            AchievementType = achievementType;

            LoadCompanyCategories();
        }

        public void SetInputValues(VCmdsAchievement info)
        {
            OrganizationIdentifier = info.OrganizationIdentifier;

            InitCompany();

            AchievementType = info.AchievementLabel;
            LoadCompanyCategories();

            LoadAchievementCategories(info.AchievementIdentifier);
        }

        public void SaveAchievementCategories(Guid achieventIdentifier)
        {
            if (!CompanyCategories.Enabled)
                return;

            var list = TAchievementClassificationSearch.Select(x => x.AchievementIdentifier == achieventIdentifier);
            var deleteList = new List<VAchievementClassification>();
            var insertList = new List<VAchievementClassification>();

            foreach (var info in list)
            {
                ListItem item = CompanyCategories.Items.FindByValue(info.CategoryIdentifier.ToString());

                if (item == null || !item.Selected)
                    deleteList.Add(info);
            }

            foreach (ListItem item in CompanyCategories.Items)
            {
                if (!item.Selected)
                    continue;

                var categoryIdentifier = Guid.Parse(item.Value);

                if (list.FirstOrDefault(i => i.CategoryIdentifier == categoryIdentifier) == null)
                {
                    insertList.Add(new VAchievementClassification
                    {
                        AchievementIdentifier = achieventIdentifier,
                        CategoryIdentifier = categoryIdentifier
                    });
                }
            }

            TAchievementClassificationStore.Insert(insertList);
            TAchievementClassificationStore.Delete(deleteList);
        }

        private void Visibility_ValueChanged(Object sender, EventArgs e)
        {
            InitCompany();
            LoadCompanyCategories();
        }

        private void InitCompany()
        {
            var org = ServiceLocator.OrganizationSearch.Get(OrganizationIdentifier);

            CompanyName.Text = org.CompanyName;
        }

        private void LoadCompanyCategories()
        {
            CompanyCategories.Items.Clear();

            var list = VAchievementCategorySearch
                .Select(x => x.OrganizationIdentifier == OrganizationIdentifier)
                .OrderBy(x => x.CategoryName)
                .ToList();

            foreach (var info in list.OrderBy(x => x.CategoryName))
            {
                if (info.AchievementLabel == AchievementType)
                {
                    ListItem item = new ListItem();
                    item.Value = info.CategoryIdentifier.ToString();
                    item.Text = info.CategoryName;

                    CompanyCategories.Items.Add(item);
                }
            }

            NoneLiteral.Visible = list.Count == 0;
        }

        private void LoadAchievementCategories(Guid achievementIdentifier)
        {
            var list = TAchievementClassificationSearch.Select(x => x.AchievementIdentifier == achievementIdentifier);

            foreach (var info in list)
            {
                ListItem item = CompanyCategories.Items.FindByValue(info.CategoryIdentifier.ToString());

                if (item != null)
                    item.Selected = true;
            }
        }
    }
}