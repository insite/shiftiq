using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class PublishEvent : Command
    {
        public DateTimeOffset? RegistrationStart { get; set; }
        public DateTimeOffset? RegistrationDeadline { get; set; }

        public PublishEvent(Guid aggregate, DateTimeOffset? registrationStart, DateTimeOffset? registrationDeadline)
        {
            AggregateIdentifier = aggregate;
            RegistrationStart = registrationStart;
            RegistrationDeadline = registrationDeadline;
        }
    }
}
