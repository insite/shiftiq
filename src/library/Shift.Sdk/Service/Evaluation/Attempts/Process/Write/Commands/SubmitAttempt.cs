using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Attempts.Write
{
    public class SubmitAttempt : Command
    {
        public string UserAgent { get; }
        public bool Grade { get; }

        public SubmitAttempt(Guid attempt, string userAgent, bool grade)
        {
            AggregateIdentifier = attempt;
            UserAgent = userAgent;
            Grade = grade;
        }
    }
}
