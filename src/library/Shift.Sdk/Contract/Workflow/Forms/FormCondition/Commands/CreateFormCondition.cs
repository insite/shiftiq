using System;

namespace Shift.Contract
{
    public class CreateFormCondition
    {
        public Guid MaskedSurveyQuestionId { get; set; }
        public Guid MaskingSurveyOptionItemId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}