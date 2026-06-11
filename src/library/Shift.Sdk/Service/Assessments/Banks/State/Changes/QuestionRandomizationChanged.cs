using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionRandomizationChanged : Change
    {
        public Guid Question { get; set; }
        public Randomization Randomization { get; set; }

        public QuestionRandomizationChanged(Guid question, Randomization randomization)
        {
            Question = question;
            Randomization = randomization;
        }
    }
}
