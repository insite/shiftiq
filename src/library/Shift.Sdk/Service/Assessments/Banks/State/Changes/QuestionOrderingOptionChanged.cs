using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class QuestionOrderingOptionChanged : Change
    {
        public Guid Question { get; set; }
        public Guid Option { get; set; }
        public ContentTitle Content { get; set; }

        public QuestionOrderingOptionChanged(Guid question, Guid option, ContentTitle content)
        {
            Question = question;
            Option = option;
            Content = content;
        }
    }
}
