namespace InSite.Persistence
{
    public interface IHistoryEntityState
    {
        TimestampModel Timestamp { get; }
        object this[string name] { get; }
        object this[int index] { get; }
        int Count { get; }
    }
}
