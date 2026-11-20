using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class RemoveExamForm : Command
    {
        public Guid Form { get; set; }

        public RemoveExamForm(Guid aggregate, Guid form)
        {
            AggregateIdentifier = aggregate;
            Form = form;
        }
    }
}
