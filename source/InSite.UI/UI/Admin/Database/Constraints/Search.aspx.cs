using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Settings.Constraints
{
    public partial class Search : SearchPage<VForeignKeyFilter>
    {
        public override string EntityName => "Constraint";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}