namespace Shift.Hub
{
    public class ApiThrottle
    {
        /// <summary>
        /// Set only when the API detects the request took an unusually long time to run. When set,
        /// the caller must wait that number of seconds before calling the method again.
        /// </summary>
        public int? BackoffSeconds { get; set; }

        public int QuotaMaximum { get; set; }
        public int QuotaRemaining { get; set; }
    }
}
