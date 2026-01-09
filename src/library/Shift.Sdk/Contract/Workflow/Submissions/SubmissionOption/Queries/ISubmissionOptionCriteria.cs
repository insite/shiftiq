using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISubmissionOptionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? SurveyQuestionIdentifier { get; set; }

        bool? ResponseOptionIsSelected { get; set; }

        int? OptionSequence { get; set; }
    }
}