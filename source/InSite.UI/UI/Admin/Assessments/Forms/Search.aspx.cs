using System;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.Forms
{
    public partial class Search : SearchPage<QBankFormFilter>
    {
        public override string EntityName => "Form";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}