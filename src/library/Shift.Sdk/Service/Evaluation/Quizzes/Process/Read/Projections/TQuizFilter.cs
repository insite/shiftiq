using System;

using Shift.Common;

namespace InSite.Application.Quizzes.Read
{
    [Serializable]
    public class TQuizFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string QuizType { get; set; }
        public string QuizNameContains { get; set; }
        public string QuizDataContains { get; set; }
        public int? TimeLimitFrom { get; set; }
        public int? TimeLimitThru { get; set; }
        public int? AttemptLimitFrom { get; set; }
        public int? AttemptLimitThru { get; set; }

        public TQuizFilter Clone()
        {
            return (TQuizFilter)MemberwiseClone();
        }
    }
}
