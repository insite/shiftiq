using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class VStatementFilter : Filter
    {
        public DateTimeOffset? StatementTimeBefore { get; set; }
        public DateTimeOffset? StatementTimeSince { get; set; }

        public string LearnerName { get; set; }
        public string ObjectUrl { get; set; }

        public VStatementFilter Clone()
        {
            return (VStatementFilter)MemberwiseClone();
        }
    }
}