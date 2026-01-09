namespace Shift.Sdk.UI
{
    public class ActiveUserGridItem
    {
        public string LoginName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string PersonCode { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string Started { get; set; }
        public string StartedHumanize { get; set; }
        public string TimeZone { get; set; }
        public bool IsOnline { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
    }
}