using System;

namespace InSite.Persistence
{
    public class TRubricConnection
    {
        public Guid RubricIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid ConnectionIdentifier { get; set; }

        public virtual TRubric Rubric { get; set; }
    }
}
