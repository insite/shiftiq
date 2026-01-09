using System;

namespace InSite.Domain.Attempts
{
    public class AttemptQuestionRubric
    {
        public Guid Identifier { get; set; }
        public decimal Points { get; set; }

        public void Set(AttemptQuestionRubric other)
        {
            Identifier = other.Identifier;
            Points = other.Points;
        }

        public AttemptQuestionRubric Clone()
        {
            return (AttemptQuestionRubric)MemberwiseClone();
        }
    }
}
