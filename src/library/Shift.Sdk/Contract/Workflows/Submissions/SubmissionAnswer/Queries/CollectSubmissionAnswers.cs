using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectSubmissionAnswers : Query<IEnumerable<SubmissionAnswerModel>>, ISubmissionAnswerCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
