using System;

namespace InSite.Application.Surveys.Read
{
    public class QSurveyCondition
    {
        public Guid MaskedSurveyQuestionIdentifier { get; set; }
        public Guid MaskingSurveyOptionItemIdentifier { get; set; }

        public virtual QSurveyOptionItem MaskingSurveyOptionItem { get; set; }
        public virtual QSurveyQuestion MaskedSurveyQuestion { get; set; }
    }
}
