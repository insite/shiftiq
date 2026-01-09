using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFormOptionItems : Query<int>, IFormOptionItemCriteria
    {
        public Guid? BranchToQuestionIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyOptionListIdentifier { get; set; }

        public string SurveyOptionItemCategory { get; set; }

        public int? SurveyOptionItemSequence { get; set; }

        public decimal? SurveyOptionItemPoints { get; set; }
    }
}