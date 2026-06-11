using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFormQuestions : Query<IEnumerable<FormQuestionModel>>, IFormQuestionCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
