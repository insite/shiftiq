using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormOptionListCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationId { get; set; }
        Guid? SurveyQuestionId { get; set; }

        int? SurveyOptionListSequence { get; set; }
    }
}