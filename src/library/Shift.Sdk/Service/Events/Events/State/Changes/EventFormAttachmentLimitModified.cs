using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventFormAttachmentLimitModified : Change
    {
        public int? Limit { get; set; }

        public EventFormAttachmentLimitModified(int? limit)
        {
            Limit = limit;
        }
    }
}
