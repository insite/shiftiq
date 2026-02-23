using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Banks
{
    public class QuestionLikertRowAdded : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid RowIdentifier { get; set; }
        public Guid? Standard { get; set; }
        public Guid[] SubStandards { get; set; }
        public ContentTitle Content { get; set; }

        public QuestionLikertRowAdded(Guid questionIdentifier, Guid rowIdentifier, Guid? standard, Guid[] subStandards, ContentTitle content)
        {
            QuestionIdentifier = questionIdentifier;
            RowIdentifier = rowIdentifier;
            Standard = standard == Guid.Empty ? null : standard;
            SubStandards = subStandards?.NullIfEmpty();
            Content = content?.IsEmpty == false ? content.Clone() : null;
        }
    }
}
