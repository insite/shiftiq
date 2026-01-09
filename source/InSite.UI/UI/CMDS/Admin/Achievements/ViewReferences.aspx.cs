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

        private string Return => Request["return"];

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                BindModelToControls();
        }

        protected void BindModelToControls()
        {
            var achievement = AchievementIdentifier.HasValue ? VCmdsAchievementSearch.Select(AchievementIdentifier.Value) : null;
            if (achievement == null)
                HttpResponseHelper.Redirect("/ui/cmds/admin/achievements/search", true);

            BindFormHeader(achievement.AchievementTitle);
            BindReferences();
            BindNavigation();
        }

        private void BindFormHeader(string qualifier)
        {
            var breadcrumbs = new List<BreadcrumbItem>();

            if (Return == "design")
            {
                breadcrumbs.Add(new BreadcrumbItem("Field Achievements", $"/ui/cmds/design/achievements/search"));
                breadcrumbs.Add(new BreadcrumbItem("Edit", $"/ui/cmds/design/achievements/edit?id={AchievementIdentifier}"));
            }
            else
            {
                breadcrumbs.Add(new BreadcrumbItem("Achievements", $"/ui/cmds/admin/achievements/search"));
                breadcrumbs.Add(new BreadcrumbItem("Edit", $"/ui/cmds/admin/achievements/edit?id={AchievementIdentifier}"));
            }

            breadcrumbs.Add(new BreadcrumbItem("View References", null, null, "active"));

            PageHelper.BindHeader(this, breadcrumbs.ToArray(), null, qualifier);
        }

        private void BindReferences()
        {
            var dependencies = VCmdsAchievementHelper.BuildReferencesText(AchievementIdentifier.Value);

            ReferenceRepeater.DataSource = dependencies.Items;
            ReferenceRepeater.DataBind();

            InstructionText.Text = dependencies.Count == 0
                ? "No references to this achievement"
                : "reference".ToQuantity(dependencies.Count) + " to this achievement";
        }

        private void BindNavigation()
        {
            CloseButton.NavigateUrl = Return == "design"
                ? $"/ui/cmds/design/achievements/edit?id={AchievementIdentifier}"
                : $"/ui/cmds/admin/achievements/edit?id={AchievementIdentifier}";
        }
    }
}