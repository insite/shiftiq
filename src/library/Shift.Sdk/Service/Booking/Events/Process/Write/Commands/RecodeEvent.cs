using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class RecodeEvent : Command
    {
        public string EventClassCode { get; set; }
        public string EventBillingCode { get; set; }

        public RecodeEvent(Guid id, string classCode, string billingCode)
        {
            AggregateIdentifier = id;
            EventClassCode = classCode;
            EventBillingCode = billingCode;
        }
    }
}
