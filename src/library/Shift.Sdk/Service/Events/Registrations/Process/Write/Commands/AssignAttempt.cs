using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignAttempt : Command
    {
        public Guid? Form { get; set; }
        public Guid Attempt { get; set; }

        public AssignAttempt(Guid aggregate, Guid? form, Guid attempt)
        {
            AggregateIdentifier = aggregate;
            Form = form;
            Attempt = attempt;
        }
    }
}
