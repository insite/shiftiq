using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ExcludeRegistrationFromT2202 : Command
    {
        public ExcludeRegistrationFromT2202(Guid registration)
        {
            AggregateIdentifier = registration;
        }
    }
}
