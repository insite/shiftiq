using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyConditionDeleted : Change
    {
        public SurveyConditionDeleted(Guid maskingItem, Guid[] maskedQuestions)
        {
            MaskingItem = maskingItem;
            MaskedQuestions = maskedQuestions;
        }

        public Guid MaskingItem { get; }
        public Guid[] MaskedQuestions { get; }
    }
}