using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionStandardChanged : Change
    {
        public Guid Question { get; set; }
        public Guid Standard { get; set; }
        public Guid[] SubStandards { get; set; }

        public QuestionStandardChanged(Guid question, Guid standard, Guid[] subStandards)
        {
            Question = question;
            Standard = standard;
            SubStandards = subStandards;
        }
    }
}
