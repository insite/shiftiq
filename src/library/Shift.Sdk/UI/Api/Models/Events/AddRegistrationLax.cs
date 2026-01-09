using System;

namespace Shift.Sdk.UI
{
    /// <summary>
    /// This is the data transfer object for lax (i.e., not strict) creation of a new event registration.
    /// </summary>
    public class AddRegistrationLax
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
        /// Uniquely identifies the learner (i.e., user/candidate) registering for the event.
        /// </summary>
        public Guid Learner { get; set; }

        /// <summary>
        /// At the time of registration, a learner may be an Apprentice or a Challenger. This is an optional property to indicate the registrant's 
        /// type when registration is first requested.
        /// </summary>
        public string LearnerRegistrantType { get; set; }

        /// <summary>
        /// An alphanumeric code that should (in theory) uniquely identify an assessment form.
        /// </summary>
        public string Assessment { get; set; }

        /// <summary>
        /// An array of (optional) accommodations.
        /// </summary>
        public GrantRegistrationAccommodation[] Accommodations { get; set; }
    }
}