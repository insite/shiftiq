using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFormCondition : Query<bool>
    {
        public Guid MaskedSurveyQuestionId { get; set; }
        public Guid MaskingSurveyOptionItemId { get; set; }
    }
}