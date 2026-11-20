using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeExamForm : Command
    {
        public Guid FromForm { get; set; }
        public Guid ToForm { get; set; }

        public ChangeExamForm(Guid aggregate, Guid fromForm, Guid toForm)
        {
            AggregateIdentifier = aggregate;
            FromForm = fromForm;
            ToForm = toForm;
        }
    }
}
