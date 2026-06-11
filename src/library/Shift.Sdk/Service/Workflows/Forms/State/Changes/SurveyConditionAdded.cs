using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyConditionAdded : Change
    {
        public SurveyConditionAdded(Guid item, Guid[] maskedQuestions)
        {
            Item = item;
            MaskedQuestions = maskedQuestions;
        }

        public Guid Item { get; }
        public Guid[] MaskedQuestions { get; }
    }
}