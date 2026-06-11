using System;

namespace InSite.Persistence.Integration.BCMail
{
    public class ExamDistributionRequest
    {
        public Guid RequestedBy { get; set; }
        public Guid RequestIdentifier { get; set; }

        public string JobCode { get; set; }
        public string JobErrors { get; set; }
        public string JobStatus { get; set; }

        public DateTimeOffset Requested { get; set; }
    }
}