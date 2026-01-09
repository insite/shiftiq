using System;
using System.Collections.Generic;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.Periods.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Records.Periods
{
    public partial class Edit : AdminBasePage
    {
        private Guid PeriodIdentifier => Guid.TryParse(Request.QueryString["period"], out var period) ? period : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/records/periods/search");

            if (!IsPostBack)
                LoadData();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = new QPeriod();
            Detail.GetInputValues(entity);

            var commands = new List<Command>();
            commands.Add(new RenamePeriod(PeriodIdentifier, entity.PeriodName));
            commands.Add(new ReschedulePeriod(PeriodIdentifier, entity.PeriodStart, entity.PeriodEnd));

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect("/ui/admin/records/periods/search");
        }

        private void LoadData()
        {
            var period = ServiceLocator.PeriodSearch.GetPeriod(PeriodIdentifier);
            if (period == null || period.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect("/ui/admin/records/periods/search");

            PageHelper.AutoBindHeader(this, null, period.PeriodName);

            Detail.SetInputValues(period);

            CancelButton.NavigateUrl = "/ui/admin/records/periods/search";
        }
    }
}
