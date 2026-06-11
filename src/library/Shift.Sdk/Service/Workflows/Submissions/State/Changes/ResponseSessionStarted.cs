using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseSessionStarted : Change
    {
        public ResponseSessionStarted(DateTimeOffset? started, bool? noStatusChange)
        {
            Started = started;
            NoStatusChange = noStatusChange;
        }

        public DateTimeOffset? Started { get; set; }
        public bool? NoStatusChange { get; set; }
    }
}
