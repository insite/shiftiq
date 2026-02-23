using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionUpgraded : Change
    {
        public Guid CurrentQuestion { get; set; }
        public Guid UpgradedQuestion { get; set; }

        public QuestionUpgraded(Guid currentQuestion, Guid upgradedQuestion)
        {
            CurrentQuestion = currentQuestion;
            UpgradedQuestion = upgradedQuestion;
        }
    }
}
