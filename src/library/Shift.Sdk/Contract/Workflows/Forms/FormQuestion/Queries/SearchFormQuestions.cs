using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFormQuestions : Query<IEnumerable<FormQuestionMatch>>, IFormQuestionCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
