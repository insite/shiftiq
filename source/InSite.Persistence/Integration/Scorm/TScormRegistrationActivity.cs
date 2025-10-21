using System;

namespace InSite.Persistence
{
    public class TScormRegistrationActivity
    {
        public virtual TScormRegistration Registration { get; set; }

        public Guid ActivityIdentifier { get; set; }
        public Guid CourseIdentifier { get; set; }
        public Guid? GradeItemIdentifier { get; set; }
        public Guid JoinIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ScormRegistrationIdentifier { get; set; }
    }
}