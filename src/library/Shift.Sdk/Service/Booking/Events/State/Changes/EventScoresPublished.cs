using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventScoresPublished : Change
    {
        public bool AlertMessageEnabled { get; set; }
        public Guid[] Registrations { get; set; }
        public string[] Warnings { get; set; }

        public EventScoresPublished(Guid[] registrations, bool alertMessageEnabled)
        {
            AlertMessageEnabled = alertMessageEnabled;
            Registrations = registrations;
        }
    }
}