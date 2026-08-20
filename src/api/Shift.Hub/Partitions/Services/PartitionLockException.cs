namespace Shift.Hub.Partitions
{
    /// <summary>
    /// Raised when the registration lock for a partition could not be acquired. The SQL error
    /// numbers below are thrown by the acquire-lock statement in <see cref="PartitionStore"/>
    /// and are the only way to tell a contention failure apart from a broken sp_getapplock call.
    /// </summary>
    public class PartitionLockException : Exception
    {
        // sp_getapplock returned -1 (timeout) or -3 (deadlock victim). The caller can retry.
        public const int RetryableErrorNumber = 51001;

        // sp_getapplock returned -2 (canceled) or -999 (parameter or call error). Retrying will not help.
        public const int FatalErrorNumber = 51002;

        public PartitionLockException(string message, bool retryable, Exception innerException)
            : base(message, innerException)
        {
            Retryable = retryable;
        }

        public bool Retryable { get; }
    }
}
