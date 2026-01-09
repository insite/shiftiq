using System;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Logs.Read
{
    [Serializable]
    public class CommandFilter : Filter
    {
        public string CommandData { get; set; }
        public string CommandClass { get; set; }
        public CommandState? CommandState { get; set; }
        public string CommandType { get; set; }

        public Guid? OriginOrganization { get; set; }
        public Guid? OriginUser { get; set; }

        public DateTimeOffsetRange SendCancelled { get; set; }
        public DateTimeOffsetRange SendCompleted { get; set; }
        public DateTimeOffsetRange SendScheduled { get; set; }
        public DateTimeOffsetRange SendStarted { get; set; }

        public DateTimeOffsetRange BookmarkAdded { get; set; }
        public DateTimeOffsetRange BookmarkExpired { get; set; }

        public bool? IsRecurring { get; set; }

        public string SendError { get; set; }

        public CommandFilter()
        {
            SendCancelled = new DateTimeOffsetRange();
            SendCompleted = new DateTimeOffsetRange();
            SendScheduled = new DateTimeOffsetRange();
            SendStarted = new DateTimeOffsetRange();
            BookmarkAdded = new DateTimeOffsetRange();
            BookmarkExpired = new DateTimeOffsetRange();
        }
    }
}
