using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormOptionItemCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? BranchToQuestionId { get; set; }
        Guid? OrganizationId { get; set; }
        Guid? SurveyOptionListId { get; set; }

        string SurveyOptionItemCategory { get; set; }

        int? SurveyOptionItemSequence { get; set; }

        decimal? SurveyOptionItemPoints { get; set; }
    }
}