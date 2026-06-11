using System;

using Shift.Common;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QEventCommentFilter : Filter
    {
        public Guid EventIdentifier { get; set; }
        public Guid AuthorIdentifier { get; set; }
    }
}
