using System;

namespace Shift.Contract
{
    public class ModifyFormOptionList
    {
        public Guid? OrganizationId { get; set; }
        public Guid SurveyOptionListId { get; set; }
        public Guid SurveyQuestionId { get; set; }

        public int SurveyOptionListSequence { get; set; }
    }
}