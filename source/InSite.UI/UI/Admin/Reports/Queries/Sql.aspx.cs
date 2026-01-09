using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Reports.Queries
{
    public partial class Sql : SearchPage<SqlFilter>
    {
        public override string EntityName => "Database Query SQL";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.Alert += (s, a) => ScreenStatus.AddMessage(a);

            PageHelper.AutoBindHeader(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsOperator)
                HttpResponseHelper.Redirect(RelativeUrl.AdminHomeUrl);
        }
    }
}