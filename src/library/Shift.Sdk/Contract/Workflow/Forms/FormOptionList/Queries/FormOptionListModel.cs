using System;

namespace Shift.Contract
{
    public partial class FormOptionListModel
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid SurveyOptionListIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }

        public int SurveyOptionListSequence { get; set; }
    }
}