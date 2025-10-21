namespace InSite.Persistence.Plugin.NCSHA
{
    [NcshaHistoryEvent("SQL Server Report", "Report Viewed", true)]
    public class SsrsViewedEvent : SsrsHistoryEvent
    {
        public SsrsViewedEvent(string code, string name)
            : base(code, name)
        {
        }
    }
}
