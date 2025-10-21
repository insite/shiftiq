using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Records.Credentials
{
    public partial class Search : SearchPage<VCredentialFilter>
    {
        public override string EntityName => "Achievement";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}