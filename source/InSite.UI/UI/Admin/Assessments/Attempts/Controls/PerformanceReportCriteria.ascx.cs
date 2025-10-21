using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class PerformanceReportCriteria : BaseUserControl
    {
        public VPerformanceReportFilter GetFilter()
        {
            var filter = new VPerformanceReportFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                FormIdentifier = FormIdentifier.Value,
                LearnerUserIdentifier = ExamCandidateID.Value.Value,
                AttemptGradedSince = AttemptGradedSince.Value,
                AttemptGradedBefore = AttemptGradedBefore.Value,
            };
            return filter;
        }
    }
}