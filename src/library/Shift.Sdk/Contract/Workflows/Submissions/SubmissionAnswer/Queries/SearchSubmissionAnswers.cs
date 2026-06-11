using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchSubmissionAnswers : Query<IEnumerable<SubmissionAnswerMatch>>, ISubmissionAnswerCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
