using System;

using InSite.Admin.Contacts.Memberships.Controls;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Sdk.UI;

namespace InSite.UI.Admin.Contacts.Memberships
{
    public partial class Search : SearchPage<MembershipFilter>
    {
        public override string EntityName => "Group Membership";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Updated += Summary_Updated;
            PageHelper.AutoBindHeader(this);
        }

        private void Summary_Updated(object sender, EventArgs e)
        {
            SummaryTables.LoadData(((MembershipSearchResultEventArgs)e).MembershipResults);
        }
    }
}