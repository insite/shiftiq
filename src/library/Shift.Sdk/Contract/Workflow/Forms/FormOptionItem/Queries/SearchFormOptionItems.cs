using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFormOptionItems : Query<IEnumerable<FormOptionItemMatch>>, IFormOptionItemCriteria
    {
        public Guid? BranchToQuestionIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyOptionListIdentifier { get; set; }

        public string SurveyOptionItemCategory { get; set; }

        public int? SurveyOptionItemSequence { get; set; }

        public decimal? SurveyOptionItemPoints { get; set; }
    }
}