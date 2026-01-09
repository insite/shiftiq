using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.NCSHA
{
    [Serializable]
    public class HistoryFilter : Shift.Common.Filter
    {
        public string[] EventTypeInclude { get; set; }
        public string[] EventTypeExclude { get; set; }
        public DateTimeOffsetRange RecordTime { get; set; } = new DateTimeOffsetRange();
        public Guid? UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
