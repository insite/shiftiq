using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchBankFormQuestionGradeitems : Query<IEnumerable<BankFormQuestionGradeitemMatch>>, IBankFormQuestionGradeitemCriteria
    {
        public Guid? GradeItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}