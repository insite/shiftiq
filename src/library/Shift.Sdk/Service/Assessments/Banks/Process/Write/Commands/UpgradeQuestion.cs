using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class UpgradeQuestion : Command
    {
        public Guid CurrentQuestion { get; set; }
        public Guid UpgradedQuestion { get; set; }

        public UpgradeQuestion(Guid bank, Guid currentQuestion, Guid upgradedQuestion)
        {
            AggregateIdentifier = bank;
            CurrentQuestion = currentQuestion;
            UpgradedQuestion = upgradedQuestion;
        }
    }
}