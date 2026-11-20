using System;

using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Home.Learning.Controls
{
    public partial class DashboardNavigation : BaseUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Organization.Toolkits.Accounts.AHRIFeatures)
            {
                MySkillsCheckLiteral.Text = "My Assessments";
                MyBadgesLink.Visible = false;
                CatalogueLink.Visible = false;
            }
        }
    }
}
