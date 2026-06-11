using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class ProgressStarted : Change
    {
        public const string Status = "Started";

        public ProgressStarted(DateTimeOffset started)
        {
            Started = started;
        }

        public DateTimeOffset Started { get; set; }
    }
}
