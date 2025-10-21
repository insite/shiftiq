using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Records.Achievements
{
    public partial class Search : SearchPage<QAchievementFilter>
    {
        public override string EntityName => "Achievement Template";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Achievement", "/ui/admin/records/achievements/define", null, null));
        }
    }
}