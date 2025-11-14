using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Jobs.Employers
{
    public partial class Search : SearchPage<VGroupEmployerFilter>
    {
        public override string EntityName => "Employers";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (IsPostBack)
                return;

            var createUrl = new ReturnUrl("").GetRedirectUrl($"/ui/admin/contacts/groups/create?type={GroupTypes.Employer}");

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Employer", createUrl, null, null));
        }
    }
}