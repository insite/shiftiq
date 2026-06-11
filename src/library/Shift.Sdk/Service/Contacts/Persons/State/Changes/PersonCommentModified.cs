using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class PersonCommentModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CommentActionType CommentActionType { get; set; }
        public PersonComment Comment { get; set; }

        public PersonCommentModified(CommentActionType commentActionType, PersonComment comment)
        {
            CommentActionType = commentActionType;
            Comment = comment;
        }
    }
}
