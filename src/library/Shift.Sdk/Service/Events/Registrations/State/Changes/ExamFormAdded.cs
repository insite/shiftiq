using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ExamFormAdded : Change
    {
        public Guid ExamForm { get; set; }
        public Guid Form { get; set; }
        public string Password { get; set; }

        public ExamFormAdded(Guid examForm, Guid form, string password)
        {
            ExamForm = examForm;
            Form = form;
            Password = password;
        }
    }
}
