using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementCertificateIssuanceChanged : Change
    {
        public bool IssuesCertificate { get; set; }

        public AchievementCertificateIssuanceChanged(bool issuesCertificate)
        {
            IssuesCertificate = issuesCertificate;
        }
    }
}
