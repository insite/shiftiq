using System;

namespace InSite.Application.Standards.Read
{
    [Serializable]
    public class CompetencyQuestion
    {
        public Guid QuestionIdentifier { get; set; }
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public decimal QuestionPoints { get; set; }
        public decimal AnswerPoints { get; set; }
    }
}
