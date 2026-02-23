using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseSessionCompleted : Change
    {
        public ResponseSessionCompleted(DateTimeOffset? completed)
        {
            Completed = completed;
        }

        public DateTimeOffset? Completed { get; set; }
    }
}
