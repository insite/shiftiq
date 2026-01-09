using System;

namespace Shift.Contract
{
    public class ModifyFormOptionList
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid SurveyOptionListIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }

        public int SurveyOptionListSequence { get; set; }
    }
}