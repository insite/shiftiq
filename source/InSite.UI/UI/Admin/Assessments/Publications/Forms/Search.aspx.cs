using System;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.Publications.Forms
{
    public partial class Search : SearchPage<VAssessmentPageFilter>
    {
        public override string EntityName => "Standalone Assessment";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                PageHelper.AutoBindHeader(this);
        }
    }
}