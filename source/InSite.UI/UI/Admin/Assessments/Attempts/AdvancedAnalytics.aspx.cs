using System;

using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments
{
    public partial class AdvancedAnalytics : AdminBasePage
    {
        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindModelToControls();
            }
        }
    }
}