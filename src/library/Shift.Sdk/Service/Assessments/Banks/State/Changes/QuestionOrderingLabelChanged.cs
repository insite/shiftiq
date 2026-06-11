using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class QuestionOrderingLabelChanged : Change
    {
        public Guid Question { get; set; }
        public bool Show { get; set; }
        public ContentTitle TopContent { get; set; }
        public ContentTitle BottomContent { get; set; }

        public QuestionOrderingLabelChanged(Guid question, bool show, ContentTitle topContent, ContentTitle bottomContent)
        {
            Question = question;
            Show = show;
            TopContent = topContent;
            BottomContent = bottomContent;
        }
    }
}
