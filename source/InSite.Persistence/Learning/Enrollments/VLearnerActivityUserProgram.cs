using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class VLearnerActivityUserProgram
    {
        public class Gradebook
        {
            public Guid GradebookIdentifier { get; set; }
            public string GradebookTitle { get; set; }
        }

        public class Credential
        {
            public Guid AchievementIdentifier { get; set; }
            public DateTimeOffset CredentialGranted { get; set; }
        }

        public Guid UserIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserGender { get; set; }
        public string UserPhone { get; set; }
        public DateTime? UserBirthdate { get; set; }
        public string PersonCode { get; set; }
        public Guid ProgramIdentifier { get; set; }
        public string ProgramName { get; set; }

        public List<Gradebook> Gradebooks { get; } = new List<Gradebook>();
        public List<Credential> Credentials { get; } = new List<Credential>();
    }
}
