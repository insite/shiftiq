using System;

namespace InSite.Domain.Attempts
{
    [Serializable]
    public class OldOption
    {
        public int Key { get; set; }
        public bool? IsTrue { get; set; }
        public decimal Points { get; set; }
    }

    [Serializable]
    public class AnswerState
    {
        public DateTimeOffset? Answered { get; set; }

        public Guid Question { get; set; }
        public Guid? Competency { get; set; }

        public decimal? QuestionPoints { get; set; }
        public decimal? AnswerPoints { get; set; }
        public decimal? AttemptScore { get; set; }

        public Guid Form { get; set; }
        public bool AttemptIsPass { get; set; }
    }
}
