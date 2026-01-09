namespace Shift.Constant
{
    public enum ExecutionState
    {
        Undefined,
        Created,    // Bookmarked
        Scheduled,  // Ready
        Started,    // Running
        Paused,     // Blocked
        Cancelled,  // Terminated
        Completed
    }
}
