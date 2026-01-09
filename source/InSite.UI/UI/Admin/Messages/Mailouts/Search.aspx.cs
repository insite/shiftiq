using System;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Messages.Mailouts
{
    public partial class Search : SearchPage<MailoutFilter>
    {
        public override string EntityName => "Mailout";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}