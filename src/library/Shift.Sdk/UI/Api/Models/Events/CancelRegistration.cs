namespace Shift.Sdk.UI
{
    /// <summary>
    /// This is the data transfer object for cancellation of an existing registration.
    /// </summary>
    public class CancelRegistration
    {
        /// <summary>
        /// An optional description of the reason for cancellation.
        /// </summary>
        public string Reason { get; set; }
    }
}