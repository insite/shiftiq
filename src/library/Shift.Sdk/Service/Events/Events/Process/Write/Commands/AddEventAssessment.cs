using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class AddEventAssessment : Command
    {
        public Guid Form { get; set; }

        public AddEventAssessment(Guid id, Guid form)
        {
            AggregateIdentifier = id;
            Form = form;
        }
    }
}
