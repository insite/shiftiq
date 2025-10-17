using System.Web;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public class EventStatisticItem
    {
        public string Text { get; set; }
        public string Html => Text.IsNotEmpty() ? HttpUtility.HtmlEncode(Text) : "<i>(None)</i>";
        public int Count { get; set; }
    }
}