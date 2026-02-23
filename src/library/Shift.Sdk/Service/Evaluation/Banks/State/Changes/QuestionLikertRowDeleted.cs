using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionLikertRowDeleted : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid RowIdentifier { get; set; }

        public QuestionLikertRowDeleted(Guid questionIdentifier, Guid rowIdentifier)
        {
            QuestionIdentifier = questionIdentifier;
            RowIdentifier = rowIdentifier;
        }
    }
}
