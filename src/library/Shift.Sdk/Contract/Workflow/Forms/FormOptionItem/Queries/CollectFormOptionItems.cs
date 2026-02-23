using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFormOptionItems : Query<IEnumerable<FormOptionItemModel>>, IFormOptionItemCriteria
    {
        public Guid? BranchToQuestionId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? SurveyOptionListId { get; set; }

        public string SurveyOptionItemCategory { get; set; }

        public int? SurveyOptionItemSequence { get; set; }

        public decimal? SurveyOptionItemPoints { get; set; }
    }
}