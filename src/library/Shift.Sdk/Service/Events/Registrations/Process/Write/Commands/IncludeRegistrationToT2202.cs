using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class IncludeRegistrationToT2202 : Command
    {
        public IncludeRegistrationToT2202(Guid registration)
        {
            AggregateIdentifier = registration;
        }
    }
}
