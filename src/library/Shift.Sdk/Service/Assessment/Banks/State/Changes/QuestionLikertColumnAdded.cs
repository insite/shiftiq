using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class QuestionLikertColumnAdded : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid ColumnIdentifier { get; set; }
        public ContentTitle Content { get; set; }

        public QuestionLikertColumnAdded(Guid questionIdentifier, Guid columnIdentifier, ContentTitle content)
        {
            QuestionIdentifier = questionIdentifier;
            ColumnIdentifier = columnIdentifier;
            Content = content?.IsEmpty == false ? content.Clone() : null;
        }
    }
}
