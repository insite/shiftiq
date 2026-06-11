using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseSessionCompleted : Change
    {
        public ResponseSessionCompleted(DateTimeOffset? completed, Guid? respondentSupervisor, bool? firstQuestionCaseSummary)
        {
            Completed = completed;
            RespondentSupervisor = respondentSupervisor;
            FirstQuestionCaseSummary = firstQuestionCaseSummary;
        }

        public DateTimeOffset? Completed { get; set; }
        public Guid? RespondentSupervisor { get; set; }
        public bool? FirstQuestionCaseSummary { get; set; }
    }
}
