using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class LimitExamTime : Command
    {
        public Guid? Form { get; set; }

        public LimitExamTime(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }

        public LimitExamTime(Guid aggregate, Guid form)
            : this(aggregate)
        {
            Form = form;
        }
    }
}
