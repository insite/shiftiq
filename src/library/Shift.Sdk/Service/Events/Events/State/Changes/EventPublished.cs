using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventPublished : Change
    {
        public DateTimeOffset? RegistrationStart { get; set; }
        public DateTimeOffset? RegistrationDeadline { get; set; }

        public EventPublished(DateTimeOffset? registrationStart, DateTimeOffset? registrationDeadline)
        {
            RegistrationStart = registrationStart;
            RegistrationDeadline = registrationDeadline;
        }
    }
}
