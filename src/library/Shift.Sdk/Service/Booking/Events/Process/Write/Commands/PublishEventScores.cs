using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class PublishEventScores : Command
    {
        public bool AlertMessageEnabled { get; set; }
        public Guid[] Registrations { get; set; }

        public PublishEventScores(Guid id, Guid[] registrations, bool alertMessageEnabled)
        {
            AggregateIdentifier = id;
            Registrations = registrations;
            AlertMessageEnabled = alertMessageEnabled;
        }
    }
}
