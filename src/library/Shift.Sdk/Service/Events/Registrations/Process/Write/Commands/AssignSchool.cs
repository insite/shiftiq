using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignSchool : Command
    {
        public Guid School { get; set; }

        public AssignSchool(Guid aggregate, Guid school)
        {
            AggregateIdentifier = aggregate;
            School = school;
        }
    }
}
