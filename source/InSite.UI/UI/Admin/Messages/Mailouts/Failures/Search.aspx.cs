using System;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Messages.Mailouts.Failures
{
    public partial class Search : SearchPage<MailoutFailureFilter>
    {
        public override string EntityName => "Mailout Failure";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}