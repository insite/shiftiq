using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseSessionStarted : Change
    {
        public ResponseSessionStarted(DateTimeOffset? started)
        {
            Started = started;
        }

        public DateTimeOffset? Started { get; set; }
    }
}
