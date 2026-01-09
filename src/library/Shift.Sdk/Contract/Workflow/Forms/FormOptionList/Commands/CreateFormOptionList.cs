using System;

namespace Shift.Contract
{
    public class CreateFormOptionList
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid SurveyOptionListIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }

        public int SurveyOptionListSequence { get; set; }
    }
}