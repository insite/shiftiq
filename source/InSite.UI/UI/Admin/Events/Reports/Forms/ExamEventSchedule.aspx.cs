using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Events.Reports.Forms
{
    public partial class ExamEventSchedule : SearchPage<VExamEventScheduleFilter>
    {
        public override string EntityName => "Exam Event Schedule";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/events/reports/dashboard");

            PageHelper.AutoBindHeader(this);
        }

        protected string GetAsAtKey()
        {
            var asAt = (DateTimeOffset)Page.GetDataItem();
            return $"{asAt.Ticks}-{asAt.Offset.Ticks}";
        }
    }
}