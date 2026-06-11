using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class RevokeAccommodation : Command
    {
        public string Type { get; set; }

        public RevokeAccommodation(Guid aggregate, string type)
        {
            AggregateIdentifier = aggregate;
            Type = type;
        }
    }
}
