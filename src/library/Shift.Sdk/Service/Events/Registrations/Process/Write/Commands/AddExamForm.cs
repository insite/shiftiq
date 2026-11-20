using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AddExamForm : Command
    {
        public Guid ExamForm { get; set; }
        public Guid Form { get; set; }

        public AddExamForm(Guid aggregate, Guid examForm, Guid form)
        {
            AggregateIdentifier = aggregate;
            ExamForm = examForm;
            Form = form;
        }
    }
}
