using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormOptionListCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? SurveyQuestionIdentifier { get; set; }

        int? SurveyOptionListSequence { get; set; }
    }
}