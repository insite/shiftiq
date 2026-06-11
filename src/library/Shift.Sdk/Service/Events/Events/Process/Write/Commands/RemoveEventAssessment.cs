using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class RemoveEventAssessment : Command
    {
        public Guid FormIdentifier { get; set; }

        public RemoveEventAssessment(Guid id, Guid form)
        {
            AggregateIdentifier = id;
            FormIdentifier = form;
        }
    }
}
