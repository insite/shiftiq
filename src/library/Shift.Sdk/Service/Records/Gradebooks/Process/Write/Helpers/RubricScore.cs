using System;

namespace InSite.Application.Records.Write
{
    public class RubricScore
    {
        public Guid QuestionId { get; set; }
        public decimal MaxPoints { get; set; }
    }
}
