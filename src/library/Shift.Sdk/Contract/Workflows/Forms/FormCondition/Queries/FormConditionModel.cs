using System;

namespace Shift.Contract
{
    public partial class FormConditionModel
    {
        public Guid MaskedSurveyQuestionId { get; set; }
        public Guid MaskingSurveyOptionItemId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}