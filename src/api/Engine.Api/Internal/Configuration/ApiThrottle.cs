namespace Engine.Api.Internal
{
    public class ApiThrottle
    {
        /// <summary>
        /// The backoff field is only set when the API detects the request took an unusually long time to run. When it is set an application must 
        /// wait that number of seconds before calling that method again.
        /// </summary>
        public int? BackoffSeconds { get; set; }

        public int QuotaMaximum { get; set; }
        public int QuotaRemaining { get; set; }
    }
}
