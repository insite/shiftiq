using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyBranchDeleted : Change
    {
        public SurveyBranchDeleted(Guid item, Guid? skipToQuestion)
        {
            Item = item;
            SkipToQuestion = skipToQuestion;
        }

        public Guid Item { get; }
        public Guid? SkipToQuestion { get; }
    }
}