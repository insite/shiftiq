using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class QuestionContentChanged : Change
    {
        public Guid Question { get; set; }
        public ContentExamQuestion Content { get; set; }

        public QuestionContentChanged(Guid question, ContentExamQuestion content)
        {
            Question = question;
            Content = content;
        }
    }
}
