using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignExamForm : Command
    {
        public Guid Form { get; set; }

        public Guid? PreviousForm { get; set; }

        public AssignExamForm(Guid aggregate, Guid form, Guid? previousForm)
        {
            AggregateIdentifier = aggregate;
            Form = form;
            PreviousForm = previousForm;
        }
    }
}
