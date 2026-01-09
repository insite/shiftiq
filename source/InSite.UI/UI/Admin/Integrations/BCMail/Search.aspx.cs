using System;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Integrations.BCMail
{
    public partial class Search : SearchPage<QEventFilter>
    {
        public override string EntityName => "Distribution";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PageHelper.AutoBindHeader(this);
        }
    }
}