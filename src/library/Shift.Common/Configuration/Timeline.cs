namespace Shift.Common
{
    public class TimelineSettings
    {
        public bool Debug { get; set; }

        public string Role { get; set; }

        public int SnapshotInterval { get; set; }

        public bool IsServer => Role == "Server";
    }
}
