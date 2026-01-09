using System;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.BanksComments
{
    public partial class Search : SearchPage<BankCommentaryFilter>
    {
        public override string EntityName => "Comment";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}