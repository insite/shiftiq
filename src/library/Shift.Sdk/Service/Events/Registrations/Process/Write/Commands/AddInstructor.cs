using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AddInstructor : Command
    {
        public Guid Instructor { get; set; }

        public AddInstructor(Guid aggregate, Guid instructor)
        {
            AggregateIdentifier = aggregate;
            Instructor = instructor;
        }
    }
}
