using System;

namespace Shift.Contract
{
    public class DeleteFormCondition
    {
        public Guid MaskedSurveyQuestionId { get; set; }
        public Guid MaskingSurveyOptionItemId { get; set; }
    }
}