using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Jobs.Candidates
{
    public partial class Search : SearchPage<PersonFilter>
    {
        public override string EntityName => "Candidates";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (IsPostBack)
                return;

            var newUrl = new ReturnUrl().GetRedirectUrl("/ui/admin/contacts/people/create?candidate=1");

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Candidate", newUrl, null, null));
        }
    }
}