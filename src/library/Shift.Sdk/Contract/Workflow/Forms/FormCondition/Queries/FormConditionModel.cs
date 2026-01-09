using System;

namespace Shift.Contract
{
    public partial class FormConditionModel
    {
        public Guid MaskedSurveyQuestionIdentifier { get; set; }
        public Guid MaskingSurveyOptionItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}