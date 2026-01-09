using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Messages
{
    public class MessageRenamed : Change
    {
        public string Name { get; set; }

        public MessageRenamed(string name)
        {
            Name = name;
        }
    }
}
