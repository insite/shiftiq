using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.NCSHA;
using InSite.UI.Layout.Admin;

using Shift.Common.Linq;

namespace InSite.Custom.Ncsha.Surveys.Forms
{
    public partial class Home : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ApplyAccessControl();

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);
                SetInputValues();
            }
        }

        private void SetInputValues()
        {
            var reports = CustomReportHelper.GetReports()
                .OrderBy(x => x.Code)
                .ToList();

            foreach (var report in reports)
                report.NavigateUrl = report.GetPreviewUrl();

            Reports.DataSource = reports.ToSearchResult();
            Reports.DataBind();
        }
    }
}