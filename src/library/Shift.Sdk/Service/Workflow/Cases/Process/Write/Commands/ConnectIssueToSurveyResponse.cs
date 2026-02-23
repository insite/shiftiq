using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ConnectIssueToSurveyResponse : Command
    {
        public Guid Response { get; set; }

        public ConnectIssueToSurveyResponse(Guid issue, Guid response)
        {
            AggregateIdentifier = issue;
            Response = response;
        }
    }
}
