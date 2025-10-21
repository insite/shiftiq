using System;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Admin.Assessments.Banks
{
    public partial class Search : SearchPage<QBankFilter>
    {
        public override string EntityName => "Bank";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem("Add New Bank", "/ui/admin/assessments/banks/create", null, null));
        }
    }
}