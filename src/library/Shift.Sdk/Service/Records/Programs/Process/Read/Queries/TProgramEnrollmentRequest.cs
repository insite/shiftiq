using System;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TProgramEnrollmentRequest
    {
        public Guid ProgramEnrollmentRequestIdentifier { get; set; }

        public Guid OrganizationIdentifier { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }

        public string LearnerUserEmail { get; set; }
        public string RequestFundingPath { get; set; }
        public string RequestStatus { get; set; }

        public DateTimeOffset RequestSubmittedAt { get; set; }
        public DateTimeOffset? RequestDecidedAt { get; set; }
        public Guid? RequestDecidedBy { get; set; }

        public decimal? RequestFee { get; set; }
        public string RequestFunderEmail { get; set; }

        public Guid? ProgramEnrollmentIdentifier { get; set; }
    }
}
