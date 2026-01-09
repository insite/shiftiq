using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Messages
{
    public class MessageRetitled : Change
    {
        public MultilingualString Title { get; set; }

        public MessageRetitled(MultilingualString title)
        {
            Title = title;
        }
    }
}
