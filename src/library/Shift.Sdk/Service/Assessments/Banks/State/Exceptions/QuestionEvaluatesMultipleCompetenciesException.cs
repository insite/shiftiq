using System;

namespace InSite.Domain.Banks
{
    [Serializable]
    public class QuestionEvaluatesMultipleCompetenciesException : Exception
    {
        public QuestionEvaluatesMultipleCompetenciesException(Guid question) 
            : base($"Question {question} evaluates multiple competencies. A question is allowed to evaluate no more than one competency.")
        { }
    }
}