using System;

namespace Shift.Common
{
    /// <summary>
    /// Represents the original context for some action in the system (e.g., operation, event, change, etc.).
    /// </summary>
    public class Origin
    {
        /// <summary>
        /// The exact date and time of the action.
        /// </summary>
        public DateTimeOffset When { get; set; }

        /// <summary>
        /// Unique identifier for an organization.
        /// </summary>
        /// <remarks>
        /// This identifies the organization account in which an action occurred. Organization 
        /// accounts are used to partition data within an enterprise account.
        /// </remarks>
        public Guid Organization { get; set; }

        /// <summary>
        /// Unique identifier for an individual.
        /// </summary>
        /// <remarks>
        /// This identifies the user account who initiated an action.
        /// </remarks>
        public Guid User { get; set; }

        /// <summary>
        /// Identifies the individual authorized to act on behalf of the user.
        /// </summary>
        /// <remarks>
        /// If a user (typicalloy a developer or administrator) initiates an action on behalf of 
        /// another user, then this property identifies the proxy. For example, if developer API 
        /// keys are used to modify the database on behalf of users in an external system, then we
        /// can identify both the user and the developer in an Origin object. For example, if John 
        /// is a user working in an external application who creates a new survey in the internal 
        /// application here, and if Alice is the developer whose API key is used to integrate the
        /// two applications, then Origin.User = John and Origin.Proxy = Alice.
        /// </remarks>
        public Guid? Proxy { get; set; }

        /// <summary>
        /// Details about the action.
        /// </summary>
        /// <remarks>
        /// This optional property answers the question, "What happened?".
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// The reason for the action.
        /// </summary>
        /// <remarks>
        /// This optional property explains why the action was initiated.
        /// </remarks>
        public string Reason { get; set; }

        /// <summary>
        /// Identifies and/or describes the source of the action.
        /// </summary>
        /// <remarks>
        /// This optional property answers the question, "Where did the action originate?". Values
        /// here might include the IP address of the user, the name of an external system, the URL
        /// of a page, the user agent of a browser, etc.
        /// </remarks>
        public string Source { get; set; }
    }
}