using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ReferenceGradebook : Command
    {
        public string Reference { get; set; }

        public ReferenceGradebook(Guid record, string reference)
        {
            AggregateIdentifier = record;
            Reference = reference;
        }
    }
}
