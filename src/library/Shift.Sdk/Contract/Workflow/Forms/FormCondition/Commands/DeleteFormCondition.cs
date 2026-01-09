using System;

namespace Shift.Contract
{
    public class DeleteFormCondition
    {
        public Guid MaskedSurveyQuestionIdentifier { get; set; }
        public Guid MaskingSurveyOptionItemIdentifier { get; set; }
    }
}