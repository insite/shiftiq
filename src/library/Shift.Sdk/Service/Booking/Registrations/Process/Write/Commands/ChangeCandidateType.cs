using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeCandidateType : Command
    {
        public string Type { get; set; }

        public ChangeCandidateType(Guid aggregate, string type)
        {
            AggregateIdentifier = aggregate;
            Type = type;
        }
    }
}
