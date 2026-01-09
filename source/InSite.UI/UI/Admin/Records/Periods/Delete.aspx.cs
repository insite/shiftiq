using System;

using InSite.Application.Periods.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Records.Periods
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private Guid PeriodIdentifier => Guid.TryParse(Request.QueryString["period"], out var period) ? period : Guid.Empty;
        private QPeriod ThisPeriod { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();

                PageHelper.AutoBindHeader(Page, null, ThisPeriod.PeriodName);
            }
                
        }
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new DeletePeriod(PeriodIdentifier));

            HttpResponseHelper.Redirect("/ui/admin/records/periods/search");
        }

        private void LoadData()
        {
            var period = ServiceLocator.PeriodSearch.GetPeriod(PeriodIdentifier);
            if (period == null || period.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect("/ui/admin/records/periods/search");

            ThisPeriod = period;

            var gradebookCount = ServiceLocator.RecordSearch.CountGradebooks(new QGradebookFilter { PeriodIdentifier = PeriodIdentifier });
            GradebookCount.Text = $"{gradebookCount:n0}";

            PeriodName.Text = $"<a href='/ui/admin/records/periods/edit?period={period.PeriodIdentifier}'>{period.PeriodName}</a>";
            PeriodText.Text = $"{period.PeriodStart:MMM d, yyyy} - {period.PeriodEnd:MMM d, yyyy}";

            NoVoid.Visible = gradebookCount > 0;
            DeleteButton.Visible = gradebookCount == 0;

            CancelButton.NavigateUrl = "/ui/admin/records/periods/search";
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            GetParent();

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent) =>
            GetParentLinkParameters(parent, null);
    }
}