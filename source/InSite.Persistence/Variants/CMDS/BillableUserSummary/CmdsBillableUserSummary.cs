using System;

namespace InSite.Persistence.Plugin.CMDS
{
    public class CmdsBillableUserSummary
    {
        public string BillingClassification { get; set; }
        public int LearnerAchievementCount_TimeSensitiveOnly { get; set; }
        public string LearnerEmail { get; set; }
        public Guid LearnerIdentifier { get; set; }
        public bool LearnerIsApproved { get; set; }
        public bool LearnerIsDisabled { get; set; }
        public string LearnerNameFirst { get; set; }
        public string LearnerNameLast { get; set; }
        public int LearnerOrganizationCount { get; set; }
        public string LearnerPhone { get; set; }
        public int LearnerProfileCount { get; set; }
        public string OrganizationCode { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string OrganizationName { get; set; }
    }
}
