using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Organizations.Write
{
    public class PublishExamEventSchedule : Command
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset Thru { get; set; }

        public PublishExamEventSchedule(Guid aggregate, DateTimeOffset from, DateTimeOffset thru)
        {
            AggregateIdentifier = aggregate;
            From = from;
            Thru = thru;
        }
    }
}
