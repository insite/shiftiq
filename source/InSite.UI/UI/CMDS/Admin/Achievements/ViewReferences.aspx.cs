using System;
using System.Collections.Generic;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Contract;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Achievements.Forms
{
    public partial class ViewDependencies : AdminBasePage, ICmdsUserControl
    {
        private Guid? AchievementIdentifier => Guid.TryParse(Request["achievement"], out var key) ? key : (Guid?)null;

        private string EditUrl => "/ui/cmds/admin/achievements/edit?id={0}";

        private string SearchUrl => "/ui/cmds/admin/achievements/search";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();
        }

        protected void BindModelToControls()
        {
            var achievement = AchievementIdentifier.HasValue
                ? VCmdsAchievementSearch.Select(AchievementIdentifier.Value)
                : null;

            if (achievement == null)
                HttpResponseHelper.Redirect(SearchUrl, true);

            BindFormHeader(achievement.AchievementTitle);
            BindReferences();
            BindNavigation();
        }

        private void BindFormHeader(string qualifier)
        {
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem("Achievements", SearchUrl),
                new BreadcrumbItem("Edit", string.Format(EditUrl, AchievementIdentifier)),
                new BreadcrumbItem("View References", null, null, "active")
            };

            PageHelper.BindHeader(this, breadcrumbs.ToArray(), null, qualifier);
        }

        private void BindReferences()
        {
            var referenceList = VCmdsAchievementHelper.BuildReferencesText(AchievementIdentifier.Value);

            ReferenceRepeater.DataSource = referenceList.Items;

            ReferenceRepeater.DataBind();

            InstructionText.Text = "Reference".ToQuantity(referenceList.Count) + " to this achievement";
        }

        private void BindNavigation()
        {
            CloseButton.NavigateUrl = string.Format(EditUrl, AchievementIdentifier);
        }
    }
}
