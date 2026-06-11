using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Messages
{
    public class ContentChanged : Change
    {
        public MultilingualString ContentText { get; set; }
        public IEnumerable<LinkItem> Links { get; set; }

        public ContentChanged(MultilingualString contentText, IEnumerable<LinkItem> links)
        {
            ContentText = contentText;
            Links = links;
        }
    }
}
