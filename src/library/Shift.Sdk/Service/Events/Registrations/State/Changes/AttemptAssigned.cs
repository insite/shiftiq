using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AttemptAssigned : Change
    {
        public Guid? Form { get; set; }
        public Guid Attempt { get; set; }

        public AttemptAssigned(Guid? form, Guid attempt)
        {
            Form = form;
            Attempt = attempt;
        }
    }
}
