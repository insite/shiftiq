using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Courses.Links
{
    public partial class Search : SearchPage<LtiLinkFilter>
    {
        public override string EntityName => "LTI Link";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                LoadSearchedResults();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Link", "/ui/admin/courses/links/create", null, null));
        }
    }
}