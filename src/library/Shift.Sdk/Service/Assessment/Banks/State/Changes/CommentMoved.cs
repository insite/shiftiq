using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class CommentMoved : Change
    {
        public Guid Comment { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CommentType Type { get; set; }

        public Guid Subject { get; set; }

        public CommentMoved(Guid comment, CommentType type, Guid subject)
        {
            Comment = comment;
            Type = type;
            Subject = subject;
        }
    }
}
