namespace Shift.Contract.Presentation
{
    public class ReactTimeZoneInfo
    {
        public string TimeZoneId { get; set; }
        public string Title { get; set; }
        public string Abbrev { get; set; }
        public string StandardCode { get; set; }
        public string DaylightCode { get; set; }
        public bool IsIANACompatible { get; set; }
    }
}