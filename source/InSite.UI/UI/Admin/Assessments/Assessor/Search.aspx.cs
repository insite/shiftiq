using System;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.Assessor
{
    public partial class Search : SearchPage<QAttemptFilter>
    {
        public override string EntityName => "Attempt";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this, null, "Grading Assessor Attempts");
        }
    }
}