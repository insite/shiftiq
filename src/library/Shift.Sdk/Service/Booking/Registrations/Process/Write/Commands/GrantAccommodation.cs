using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class GrantAccommodation : Command
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public int? TimeExtension { get; set; }

        public GrantAccommodation(Guid aggregate, string type, string name, int? timeExtension)
        {
            AggregateIdentifier = aggregate;
            Type = type;
            Name = name;
            TimeExtension = timeExtension;
        }
    }
}
