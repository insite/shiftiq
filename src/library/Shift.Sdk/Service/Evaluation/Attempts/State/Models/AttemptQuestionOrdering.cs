using System;

namespace InSite.Domain.Attempts
{
    public class AttemptQuestionOrdering : AttemptQuestion
    {
        public string TopLabel { get; set; }
        public string BottomLabel { get; set; }

        public AttemptOption[] Options { get; set; }
        public AttemptQuestionOrderingSolution[] Solutions { get; set; }
    }

    public class AttemptQuestionOrderingSolution
    {
        public Guid Identifier { get; set; }
        public int[] OptionsOrder { get; set; }
        public decimal Points { get; set; }
        public decimal? CutScore { get; set; }
    }
}
