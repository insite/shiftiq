using System;

namespace InSite.Application.Records.Read
{
    public class QProgressHistory
    {
        public Guid AggregateIdentifier { get; set; }
        public Guid ChangeBy { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string ProgressType { get; set; }

        public int AggregateVersion { get; set; }

        public DateTimeOffset ChangeTime { get; set; }
        public DateTimeOffset? ProgressTime { get; set; }
    }
}
