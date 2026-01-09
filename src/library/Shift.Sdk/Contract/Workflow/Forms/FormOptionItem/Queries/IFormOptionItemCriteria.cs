using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormOptionItemCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? BranchToQuestionIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? SurveyOptionListIdentifier { get; set; }

        string SurveyOptionItemCategory { get; set; }

        int? SurveyOptionItemSequence { get; set; }

        decimal? SurveyOptionItemPoints { get; set; }
    }
}