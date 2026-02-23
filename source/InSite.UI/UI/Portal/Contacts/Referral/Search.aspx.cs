using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Portal.Contacts.Referral
{
    public partial class Search : SearchPage<PersonFilter>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Identity.IsGranted("Portal/Contacts", DataAccess.Update))
                PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Person", "/ui/portal/contacts/referral/create", null, null));
            else
                PageHelper.AutoBindHeader(this);
        }
    }
}