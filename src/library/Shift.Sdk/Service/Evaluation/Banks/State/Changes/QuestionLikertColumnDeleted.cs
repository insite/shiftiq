using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionLikertColumnDeleted : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid ColumnIdentifier { get; set; }

        public QuestionLikertColumnDeleted(Guid questionIdentifier, Guid columnIdentifier)
        {
            QuestionIdentifier = questionIdentifier;
            ColumnIdentifier = columnIdentifier;
        }
    }
}
