using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AttemptAssigned : Change
    {
        public Guid Attempt { get; set; }

        public AttemptAssigned(Guid attempt)
        {
            Attempt = attempt;
        }
    }
}
