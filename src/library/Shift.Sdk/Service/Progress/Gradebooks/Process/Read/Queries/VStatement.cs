using System;

namespace InSite.Application.Records.Read
{
    public class VStatement
    {
        public Guid StatementIdentifier { get; set; }

        public string ActorEmail { get; set; }
        public string ActorName { get; set; }
        public string ObjectId { get; set; }
        public string StatementData { get; set; }
        public string StatementProvider { get; set; }
        public string VerbDisplay { get; set; }

        public DateTimeOffset StatementTime { get; set; }

        public string LearnerEmail { get; set; }
        public string LearnerName { get; set; }
    }
}
