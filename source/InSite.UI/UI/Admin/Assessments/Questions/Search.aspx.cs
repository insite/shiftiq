using System;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.Questions
{
    public partial class Search : SearchPage<QBankQuestionFilter>
    {
        public override string EntityName => "Question";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}