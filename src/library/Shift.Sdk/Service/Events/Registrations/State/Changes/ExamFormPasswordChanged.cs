using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ExamFormPasswordChanged : Change
    {
        public Guid Form { get; set; }
        public string Password { get; set; }

        public ExamFormPasswordChanged(Guid form, string password)
        {
            Form = form;
            Password = password;
        }
    }
}
