using System;

namespace Shift.Contract
{
    public class ModifyFormCondition
    {
        public Guid MaskedSurveyQuestionIdentifier { get; set; }
        public Guid MaskingSurveyOptionItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}