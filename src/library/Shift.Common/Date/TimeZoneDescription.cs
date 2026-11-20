namespace Shift.Common
{
    public class TimeZoneDescription
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Generic { get; set; }
        public string Standard { get; set; }
        public string Daylight { get; set; }
        public bool IsIANACompatible { get; set; }
    }
}
