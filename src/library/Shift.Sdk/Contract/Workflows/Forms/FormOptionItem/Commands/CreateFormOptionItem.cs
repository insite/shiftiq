using System;

namespace Shift.Contract
{
    public class CreateFormOptionItem
    {
        public Guid? BranchToQuestionId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid SurveyOptionItemId { get; set; }
        public Guid SurveyOptionListId { get; set; }

        public string SurveyOptionItemCategory { get; set; }

        public int SurveyOptionItemSequence { get; set; }

        public decimal? SurveyOptionItemPoints { get; set; }
    }
}