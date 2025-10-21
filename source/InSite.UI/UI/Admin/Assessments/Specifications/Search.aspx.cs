using System;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.Specifications
{
    public partial class Search : SearchPage<QBankSpecificationFilter>
    {
        public override string EntityName => "Specification";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}