using System;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Contacts.MembershipReasons
{
    public partial class Search : SearchPage<QMembershipReasonFilter>
    {
        public override string EntityName => "Membership Reason";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}