using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountBankFormQuestionGradeitems : Query<int>, IBankFormQuestionGradeitemCriteria
    {
        public Guid? GradeItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}