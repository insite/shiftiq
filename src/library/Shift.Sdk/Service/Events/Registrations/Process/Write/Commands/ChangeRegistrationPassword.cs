using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeRegistrationPassword : Command
    {
        public string Password { get; set; }

        public ChangeRegistrationPassword(Guid registration, string password)
        {
            AggregateIdentifier = registration;
            Password = password;
        }
    }
}
