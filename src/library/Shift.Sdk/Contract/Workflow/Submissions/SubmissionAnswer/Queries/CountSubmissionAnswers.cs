using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSubmissionAnswers : Query<int>, ISubmissionAnswerCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RespondentUserIdentifier { get; set; }

        public string ResponseAnswerText { get; set; }
    }
}