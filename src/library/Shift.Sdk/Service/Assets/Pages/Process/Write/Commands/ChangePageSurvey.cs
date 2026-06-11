using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageSurvey : Command
    {
        public Guid? Survey { get; set; }
        public ChangePageSurvey(Guid page, Guid? survey)
        {
            AggregateIdentifier = page;
            Survey = survey;
        }
    }
}
