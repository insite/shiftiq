using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyOptionList : Command
    {
        public DeleteSurveyOptionList(Guid form, Guid list)
        {
            AggregateIdentifier = form;
            List = list;
        }

        public Guid List { get; }
    }
}