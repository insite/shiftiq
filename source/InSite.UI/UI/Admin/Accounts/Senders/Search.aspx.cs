using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Accounts.Senders
{
    public partial class Search : SearchPage<TSenderFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Sender", "/ui/admin/accounts/senders/create", null, null));
        }
    }
}