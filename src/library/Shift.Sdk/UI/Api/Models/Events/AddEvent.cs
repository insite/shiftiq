using System;

namespace Shift.Sdk.UI
{
    /// <summary>
    /// This is the data transfer object for creation of a new event.
    /// </summary>
    public class AddEvent
    {
        /// <summary>
        /// Uniquely identifies the physical location for the venue hosting the event.
        /// </summary>
        public Guid EventVenue { get; set; }

        /// <summary>
        /// The date and time (including time zone) for the scheduled start of the event.
        /// </summary>
        public DateTimeOffset EventStart { get; set; }

        /// <summary>
        /// The exam type, used in case a new exam need to be created
        /// </summary>
        public string EventExamType { get; set; }

        /// <summary>
        /// The exam format, used in case a new exam need to be created
        /// </summary>
        public string EventExamFormat { get; set; }

        /// <summary>
        /// The exam billing code, used in case a new exam need to be created
        /// </summary>
        public string EventBillingCode { get; set; }

        /// <summary>
        /// The maximum number of candidate registrations permitted in the event.
        /// </summary>
        public int? RegistrationLimit { get; set; }
    }
}