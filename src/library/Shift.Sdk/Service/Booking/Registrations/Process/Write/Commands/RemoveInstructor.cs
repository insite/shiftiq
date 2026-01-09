using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class RemoveInstructor : Command
    {
        public Guid Instructor { get; set; }

        public RemoveInstructor(Guid aggregate, Guid instructor)
        {
            AggregateIdentifier = aggregate;
            Instructor = instructor;
        }
    }
}
