using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignEmployer : Command
    {
        public Guid? Employer { get; set; }

        public AssignEmployer(Guid aggregate, Guid? employer)
        {
            AggregateIdentifier = aggregate;
            Employer = employer;
        }
    }
}
