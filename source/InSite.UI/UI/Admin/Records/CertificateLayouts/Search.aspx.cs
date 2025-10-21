using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Records.AchievementLayouts
{
    public partial class Search1 : SearchPage<TCertificateLayoutFilter>
    {
        public override string EntityName => "Achievement Layout";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Achievement Layout", "/ui/admin/records/achievement-layouts/create", null, null));
        }
    }
}