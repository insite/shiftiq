using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFormCondition : Query<FormConditionModel>
    {
        public Guid MaskedSurveyQuestionIdentifier { get; set; }
        public Guid MaskingSurveyOptionItemIdentifier { get; set; }
    }
}