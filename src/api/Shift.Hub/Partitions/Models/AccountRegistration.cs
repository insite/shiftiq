namespace Shift.Hub.Partitions
{
    public class AccountRegistration
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? Number { get; set; }
        public DateTimeOffset? OpenedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
    }
}
