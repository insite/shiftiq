namespace InSite.Domain.Attempts
{
    public class AttemptQuestionMatch : AttemptQuestion
    {
        public AttemptQuestionMatchPair[] Pairs { get; set; }
        public string[] Distractors { get; set; }
    }

    public class AttemptQuestionMatchPair
    {
        public string LeftText { get; set; }
        public string RightText { get; set; }
        public decimal Points { get; set; }
    }
}
