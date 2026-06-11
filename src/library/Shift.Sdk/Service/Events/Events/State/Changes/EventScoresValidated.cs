using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventScoresValidated : Change
    {
        public Guid[] Registrations { get; set; }

        public EventScoresValidated(Guid[] registrations)
        {
            Registrations = registrations;
        }
    }
}