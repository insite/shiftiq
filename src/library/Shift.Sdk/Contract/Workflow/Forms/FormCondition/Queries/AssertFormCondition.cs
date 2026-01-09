using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertFormCondition : Query<bool>
    {
        public Guid MaskedSurveyQuestionIdentifier { get; set; }
        public Guid MaskingSurveyOptionItemIdentifier { get; set; }
    }
}