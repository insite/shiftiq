using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class ScormStatementFilter : Filter
    {
        public Guid? GradebookIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }
        public Guid? StatementIdentifier { get; set; }

        public string CourseHook { get; set; }
        public string ObjectDefinitionName { get; set; }
    }
}
