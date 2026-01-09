using System;

using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Logbooks
{
    public partial class AddUsers : AdminBasePage, IHasParentLinkParameters
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddUsersControl.SearchUrl = "/ui/admin/records/logbooks/search";
            AddUsersControl.OutlineUrl = "/ui/admin/records/logbooks/outline";
            AddUsersControl.ScreenUrl = "/ui/admin/records/logbooks/add-users";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(AddUsersControl.SearchUrl);

            PageHelper.AutoBindHeader(Page);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={AddUsersControl.JournalSetupIdentifier}&panel=users"
                : null;
        }
    }
}