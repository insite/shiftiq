using System;

namespace Shift.Contract
{
    public partial class FormOptionItemModel
    {
        public Guid? BranchToQuestionIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid SurveyOptionItemIdentifier { get; set; }
        public Guid SurveyOptionListIdentifier { get; set; }

        public string SurveyOptionItemCategory { get; set; }

        public int SurveyOptionItemSequence { get; set; }

        public decimal? SurveyOptionItemPoints { get; set; }
    }
}