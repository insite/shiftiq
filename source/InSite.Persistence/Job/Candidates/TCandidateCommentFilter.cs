using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TCandidateCommentFilter : Filter
    {
        public Guid? AuthorContactId { get; set; }
        public Guid? SubjectContactId { get; set; }
    }
}
