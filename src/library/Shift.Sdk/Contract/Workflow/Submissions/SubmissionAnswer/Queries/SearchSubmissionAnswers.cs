using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchSubmissionAnswers : Query<IEnumerable<SubmissionAnswerMatch>>, ISubmissionAnswerCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RespondentUserIdentifier { get; set; }

        public string ResponseAnswerText { get; set; }
    }
}