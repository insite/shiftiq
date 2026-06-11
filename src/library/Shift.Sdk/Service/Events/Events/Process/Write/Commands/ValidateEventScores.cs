using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ValidateEventScores : Command
    {
        public Guid[] Registrations { get; set; }

        public ValidateEventScores(Guid id, Guid[] registrations)
        {
            AggregateIdentifier = id;
            Registrations = registrations;
        }
    }
}
