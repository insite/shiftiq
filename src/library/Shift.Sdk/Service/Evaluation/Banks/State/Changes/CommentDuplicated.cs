using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class CommentDuplicated : Change
    {
        public Guid SourceComment { get; set; }
        public Guid SourceSubject { get; set; }

        public Guid DestinationComment { get; set; }
        public Guid DestinationSubject { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CommentType DestinationType { get; set; }

        public CommentDuplicated(Guid sourceComment, Guid sourceSubject, Guid destinationComment, Guid destinationSubject, CommentType destinationType)
        {
            SourceComment = sourceComment;
            SourceSubject = sourceSubject;
            DestinationSubject = destinationSubject;
            DestinationComment = destinationComment;
            DestinationType = destinationType;
        }
    }
}
