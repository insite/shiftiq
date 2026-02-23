using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyBranch : Command
    {
        public DeleteSurveyBranch(Guid form, Guid fromItem, Guid toQuestion)
        {
            AggregateIdentifier = form;
            FromItem = fromItem;
            ToQuestion = toQuestion;
        }

        public Guid FromItem { get; }
        public Guid ToQuestion { get; }
    }
}