using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class ChangeAchievementCertificateIssuance : Command
    {
        public bool IssuesCertificate { get; set; }

        public ChangeAchievementCertificateIssuance(Guid achievement, bool issuesCertificate)
        {
            AggregateIdentifier = achievement;
            IssuesCertificate = issuesCertificate;
        }
    }
}
