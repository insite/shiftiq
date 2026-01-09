using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ISubmissionAnswerCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? RespondentUserIdentifier { get; set; }

        string ResponseAnswerText { get; set; }
    }
}