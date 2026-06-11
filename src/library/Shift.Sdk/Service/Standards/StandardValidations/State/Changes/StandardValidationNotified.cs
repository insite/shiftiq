using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardValidationNotified : Change
    {
        public DateTimeOffset? Date { get; }

        public StandardValidationNotified(DateTimeOffset? date)
        {
            Date = date;
        }
    }
}
