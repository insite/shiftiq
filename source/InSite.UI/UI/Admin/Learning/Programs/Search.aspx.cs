using System;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Admin.Records.Programs
{
    public partial class Search : SearchPage<TProgramFilter>
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/search";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        public override string EntityName => "Program";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Program", "/ui/admin/learning/programs/create", null, null));
        }
    }
}