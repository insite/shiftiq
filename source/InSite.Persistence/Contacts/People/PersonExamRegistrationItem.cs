using System;

namespace InSite.Persistence
{
    [Serializable]
    public class PersonExamRegistrationItem
    {
        public Guid UserIdentifier { get; set; }

        public string Email { get; set; }
        public string EmailAlternate { get; set; }

        public Guid? EmployerIdentifier { get; set; }
        public string EmployerName { get; set; }
        public string FullName { get; set; }
        public string PersonCode { get; set; }
    }
}
