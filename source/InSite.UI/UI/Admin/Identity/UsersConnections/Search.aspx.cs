using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Identity.UsersConnections
{
    public partial class Search : SearchPage<QUserConnectionFilter>
    {
        public override string EntityName => "User Connection";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}