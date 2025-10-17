namespace InSite.Domain.Attempts
{
    public class AttemptQuestionComposed : AttemptQuestion
    {
        public AttemptQuestionRubric Rubric { get; set; }
    }

    public class AttemptQuestionComposedVoice : AttemptQuestionComposed
    {
        public int TimeLimit { get; set; }
        public int AttemptLimit { get; set; }
    }

}
