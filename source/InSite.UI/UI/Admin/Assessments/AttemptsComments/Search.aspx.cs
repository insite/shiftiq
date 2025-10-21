using System;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.AttemptsComments
{
    public partial class Search : SearchPage<QAttemptCommentaryFilter>
    {
        public override string EntityName => "Attempt Comment";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}