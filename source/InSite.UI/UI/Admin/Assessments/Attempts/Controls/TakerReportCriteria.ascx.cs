using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class TakerReportCriteria : BaseUserControl
    {
        public QAttemptFilter GetFilter()
        {
            var filter = new QAttemptFilter
            {
                FormOrganizationIdentifier = Organization.Identifier,
                FormIdentifier = FormIdentifier.Value,
                LearnerUserIdentifier = ExamCandidateID.Value.Value,
                AttemptGradedSince = AttemptGradedSince.Value,
                AttemptGradedBefore = AttemptGradedBefore.Value,
            };
            return filter;
        }
    }
}