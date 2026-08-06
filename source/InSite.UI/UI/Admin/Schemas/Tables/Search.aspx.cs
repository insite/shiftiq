using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Settings.Tables
{
    public partial class Search : SearchPage<VTableFilter>
    {
        public override string EntityName => "Table";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}