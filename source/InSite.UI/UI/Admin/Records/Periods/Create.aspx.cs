using System;
using System.Web.UI;

using InSite.Application.Periods.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Periods
{
    public partial class Create : AdminBasePage
    {
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
            {
                PageHelper.AutoBindHeader(this);
                CancelButton.NavigateUrl = "/ui/admin/records/periods/search";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = new QPeriod();
            Detail.GetInputValues(entity);

            var periodId = UniqueIdentifier.Create();
            var create = new CreatePeriod(periodId, Organization.Identifier, entity.PeriodName, entity.PeriodStart, entity.PeriodEnd);
            ServiceLocator.SendCommand(create);

            HttpResponseHelper.Redirect($"/ui/admin/records/periods/edit?id={periodId}");
        }
    }
}
