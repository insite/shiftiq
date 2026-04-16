using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Responses.Write
{
    public class CompleteResponseSession : Command
    {
        public DateTimeOffset? Completed { get; set; }
        public Guid? RespondentSupervisor { get; set; }
        public bool FirstQuestionCaseSummary { get; set; }

        public CompleteResponseSession(Guid session, DateTimeOffset? completed, Guid? respondentSupervisor, bool firstQuestionCaseSummary)
        {
            AggregateIdentifier = session;
            Completed = completed;
            RespondentSupervisor = respondentSupervisor;
            FirstQuestionCaseSummary = firstQuestionCaseSummary;
        }
    }
}