using System;

namespace Shift.Contract
{
    public partial class FormConditionMatch
    {
        public Guid MaskedSurveyQuestionId { get; set; }
        public Guid MaskingSurveyOptionItemId { get; set; }
    }
}