using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class QuestionLikertColumnChanged : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid ColumnIdentifier { get; set; }
        public ContentTitle Content { get; set; }

        public QuestionLikertColumnChanged(Guid questionIdentifier, Guid columnIdentifier, ContentTitle content)
        {
            QuestionIdentifier = questionIdentifier;
            ColumnIdentifier = columnIdentifier;
            Content = content?.IsEmpty == false ? content.Clone() : null;
        }
    }
}
