using System;

using InSite.Common.Web;
using InSite.UI.Admin.Assessments.Attempts.Controls.SkillsCheckReport;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Assessments.Attempts
{
    public partial class SkillsCheckReport : AdminBasePage
    {
        private Guid AttemptId => Guid.TryParse(Request.QueryString["attempt"], out var attemptId) ? attemptId : Guid.Empty;
        private Guid ManagerUserId => Guid.TryParse(Request.QueryString["manager"], out var managerId) ? managerId : Guid.Empty;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsOperator)
                return;

            var pdf = SkillsCheckReportControl.GetPdf(this, AttemptId, ManagerUserId, Organization.Identifier, User.TimeZone);
            if (pdf == null)
                return;

            Response.SendFile("test_skillscheck_report.pdf", pdf);
        }
    }
}