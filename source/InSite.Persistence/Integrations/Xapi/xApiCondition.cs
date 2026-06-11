using System;

namespace InSite.Persistence
{
    public class xApiCondition
    {
        public Guid ActivityIdentifier { get; set; }

        public Guid GradebookIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }

        public string StatementWhenVerb { get; set; }
        public string StatementWhenObject { get; set; }
        public string StatementThenCommand { get; set; }
    }
}