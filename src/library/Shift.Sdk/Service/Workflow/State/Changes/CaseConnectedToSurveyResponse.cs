using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseConnectedToSurveyResponse : Change
    {
        public Guid Response { get; set; }

        public CaseConnectedToSurveyResponse(Guid response)
        {
            Response = response;
        }
    }
}
