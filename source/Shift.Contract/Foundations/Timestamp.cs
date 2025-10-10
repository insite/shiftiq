using System;

namespace Shift.Contract
{
    /// <summary>
    /// An actor represents an individual person or group or system that performs actions through the UI and/or API.
    /// </summary>
    public class Actor
    {
        public Guid Identifier { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class Timestamp
    {
        /// <summary>
        /// The exact date and time of some action (i.e. some event that changes the state of the system). It is intended to 
        /// answer the question, "When did this happen?".
        /// </summary>
        public DateTimeOffset When { get; set; }

        /// <summary>
        /// Details about the action that occurred. It is intended to answer the question, "What happened?".
        /// </summary>
        public string What { get; set; }

        /// <summary>
        /// Where did the action originate? It is intended to answer the question, "Where did the action come from?". A user's IP
        /// address may be a good choice for values here.
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// The identity of the actor who caused the action to occur. It is intended to answer the question, "Who did this?".
        /// </summary>
        public Actor Who { get; set; }

        /// <summary>
        /// The reason for the action. It is intended to answer the question, "Why was this done?" 
        /// </summary>
        public string Why { get; set; }

        /// <summary>
        /// How was the change made? For example, was it made via the API or the UI? If it was made via the API, what was the API
        /// token used? If it was made via the UI, what was the URL of the page that was used to make the change, and/or what was
        /// the user agent of the browser that was used to make the change?
        /// </summary>
        public string How { get; set; }
    }
}