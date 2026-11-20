using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeExamFormPassword : Command
    {
        public Guid Form { get; set; }
        public string Password { get; set; }

        public ChangeExamFormPassword(Guid registration, Guid form, string password)
        {
            AggregateIdentifier = registration;
            Form = form;
            Password = password;
        }
    }
}
