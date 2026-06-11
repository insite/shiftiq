using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class ScormRegistrationFilter : Filter
    {
        public Guid? CourseIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string CourseHook { get; set; }
        public string RegistrationCompletion { get; set; }
    }
}