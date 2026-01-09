using System;

namespace Shift.Contract
{
    public partial class FormConditionMatch
    {
        public Guid MaskedSurveyQuestionIdentifier { get; set; }
        public Guid MaskingSurveyOptionItemIdentifier { get; set; }
    }
}