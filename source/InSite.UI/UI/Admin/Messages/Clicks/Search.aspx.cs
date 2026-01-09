using System;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Messages.Clicks
{
    public partial class Search : SearchPage<VClickFilter>
    {
        public override string EntityName => "Click";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}