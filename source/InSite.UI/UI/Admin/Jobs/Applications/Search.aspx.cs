using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Jobs.Applications
{
    public partial class Search : SearchPage<TApplicationFilter>
    {
        public override string EntityName => "Job applications";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Job application", "/ui/admin/jobs/applications/create", null, null));
        }
    }
}