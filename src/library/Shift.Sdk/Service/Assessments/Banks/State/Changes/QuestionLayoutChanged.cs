using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionLayoutChanged : Change
    {
        public Guid Question { get; set; }
        public OptionLayout Layout { get; set; }

        public QuestionLayoutChanged(Guid question, OptionLayout layout)
        {
            Question = question;
            Layout = layout;
        }
    }
}
