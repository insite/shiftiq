using System;
using System.Web;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public class HtmlDataItem
    {
        public string When { get; }

        public string Who { get; }

        public string What { get; }

        public HtmlDataItem(HistoryEntity change, TimeZoneInfo timeZone)
        {
            When = TimeZones.Format(change.Date, timeZone);
            Who = HttpUtility.HtmlEncode(change.UserName);
            What = Markdown.ToHtml(change.BuildDescription());
        }
    }
}