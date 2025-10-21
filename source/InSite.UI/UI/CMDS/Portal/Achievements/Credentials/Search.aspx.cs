using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.UI.CMDS.Portal.Achievements.Credentials
{
    public partial class Search : AdminBasePage, ICmdsUserControl
    {
        private Guid PersonID => !string.IsNullOrEmpty(Page.Request["userID"]) && Guid.TryParse(Request["userID"], out Guid personID)
            ? personID : User.UserIdentifier;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButton_Click;
            ClearButton.Click += ClearButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();

                CriteriaSection.IsSelected = false;
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            LoadData();

            CriteriaSection.IsSelected = false;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            AchievementType.ClearSelection();
            AchievementTitle.Text = null;
            ProgressionStatus.ClearSelection();

            CompletionDateSince.Value = null;
            CompletionDateBefore.Value = null;
            ExpirationDateSince.Value = null;
            ExpirationDateBefore.Value = null;
        }

        private void LoadData()
        {
            var user = UserSearch.Select(PersonID);
            if (user == null)
                HttpResponseHelper.Redirect("/ui/cmds/portal/achievements/credentials/search");

            var filter = new VCmdsCredentialFilter
            {
                UserIdentifier = PersonID,
                IsCompetencyTraining = true,
                AchievementType = AchievementType.Value,
                NotAchievementType = "Course",
                AchievementTitle = AchievementTitle.Text,
                ProgressionStatus = ProgressionStatus.Value
            };

            filter.CompletionDate.Since = CompletionDateSince.Value;
            filter.CompletionDate.Before = CompletionDateBefore.Value;
            filter.ExpirationDate.Since = ExpirationDateSince.Value;
            filter.ExpirationDate.Before = ExpirationDateBefore.Value;

            var groupBy = GroupBy.Value;

            var creatorUrl = "/ui/cmds/portal/achievements/credentials/create";

            if (PersonID != User.Identifier)
                creatorUrl += "?userid=" + PersonID;

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New", creatorUrl), user.FullName);

            AchievementsGrid.LoadData(filter, groupBy);
            AchievementsTab.Visible = AchievementsGrid.GroupsCount > 0;
            AchievementsTab.IsSelected = AchievementsTab.Visible;

            filter.IsCompetencyTraining = false;
            ExperiencesGrid.LoadData(filter, groupBy);
            ExperiencesTab.Visible = ExperiencesGrid.GroupsCount > 0;
            ExperiencesTab.IsSelected = !AchievementsTab.IsSelected && ExperiencesTab.Visible;

            CertificatesTab.Visible = !filter.AchievementType.HasValue();
            if (CertificatesTab.Visible)
            {
                CertificatesGrid.LoadData(PersonID);
                CertificatesTab.Visible = CertificatesGrid.RowCount > 0;
                CertificatesTab.IsSelected = !AchievementsTab.IsSelected && !ExperiencesTab.IsSelected;
                CertificatesPanelTitle.InnerHtml = $"Certificates <span class=\"form-text\">({CertificatesGrid.RowCount})</span>";
            }
        }
    }
}
