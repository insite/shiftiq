using System;

namespace InSite.Application.Records.Read
{
    // An enrollment request joined to the learner name and program name, for the approver queue. The
    // queue defaults to the pending ones but can show any state, so the row carries its own status and
    // decision stamp.
    [Serializable]
    public class ProgramEnrollmentRequestListItem
    {
        public Guid ProgramEnrollmentRequestIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }

        public string LearnerName { get; set; }
        public string LearnerUserEmail { get; set; }
        public string ProgramName { get; set; }

        public string RequestFundingPath { get; set; }
        public string RequestStatus { get; set; }
        public decimal? RequestFee { get; set; }
        public DateTimeOffset RequestSubmittedAt { get; set; }
        public DateTimeOffset? RequestDecidedAt { get; set; }
    }
}
