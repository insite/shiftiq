using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageAssessment : Command
    {
        public Guid? Assessment { get; set; }
        public ChangePageAssessment(Guid page, Guid? assessment)
        {
            AggregateIdentifier = page;
            Assessment = assessment;
        }
    }
}
