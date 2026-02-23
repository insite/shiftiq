using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveFormCondition : Query<FormConditionModel>
    {
        public Guid MaskedSurveyQuestionId { get; set; }
        public Guid MaskingSurveyOptionItemId { get; set; }
    }
}