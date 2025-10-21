using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Accounts.Developers
{
    public partial class Search : SearchPage<QPersonSecretFilter>
    {
        public override string EntityName => "Developer";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                LoadSearchedResults();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Developer", "/ui/admin/accounts/developers/create", null, null));
        }
    }
}