using System;

namespace Shift.Contract
{
    public class ModifyFormCondition
    {
        public Guid MaskedSurveyQuestionId { get; set; }
        public Guid MaskingSurveyOptionItemId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}