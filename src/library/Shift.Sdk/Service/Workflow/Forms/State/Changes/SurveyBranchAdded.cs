using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyBranchAdded : Change
    {
        public SurveyBranchAdded(Guid fromItem, Guid? toQuestion)
        {
            FromItem = fromItem;
            ToQuestion = toQuestion;
        }

        public Guid FromItem { get; }
        public Guid? ToQuestion { get; }
    }
}