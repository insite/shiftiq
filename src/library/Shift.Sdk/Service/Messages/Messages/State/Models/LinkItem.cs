using Shift.Common;

namespace InSite.Domain.Messages
{
    public class LinkItem
    {
        public LinkItem()
        {

        }

        public LinkItem(string tag, string href, string text)
        {
            HtmlTag = tag;
            Href = href;
            Text = text.IsEmpty() ? href : text;
        }

        public string HtmlTag { get; set; }
        public string Href { get; set; }
        public string Text { get; set; }
    }
}