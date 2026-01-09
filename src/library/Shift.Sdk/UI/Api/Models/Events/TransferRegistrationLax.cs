using System;

namespace Shift.Sdk.UI
{
    /// <summary>
    /// This is the data transfer object for lax (i.e., not strict) movement of an existing registration from one event to another event.
    /// </summary>
    public class TransferRegistrationLax
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
        /// Uniquely identifies the learner (i.e., user/candidate) registering for the event.
        /// </summary>
        public Guid Learner { get; set; }

        /// <summary>
        /// An optional description of the reason for the transfer.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// An array of (optional) accommodations.
        /// </summary>
        public GrantRegistrationAccommodation[] Accommodations { get; set; }
    }
}