using System;

namespace InSite.Domain.CourseObjects
{
    [Serializable]
    public class AnswerResult
    {
        public Guid Form { get; set; }
        public decimal? AttemptScore { get; set; }
        public bool AttemptIsPass { get; set; }
        public bool AnswerIsCorrect { get; set; }
    }
}