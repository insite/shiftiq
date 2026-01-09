using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankFormQuestionGradeitemCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? GradeItemIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
    }
}