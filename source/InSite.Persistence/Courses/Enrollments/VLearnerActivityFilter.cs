using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VLearnerActivityFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid[] MembershipStatusItemIdentifiers { get; set; }
        public Guid[] EmployerGroupIdentifiers { get; set; }
        public Guid[] ProgramIdentifiers { get; set; }
        public Guid[] GradebookIdentifiers { get; set; }

        public string UserLastName { get; set; }
        public string UserFirstName { get; set; }
        public string UserEmail { get; set; }
        public string PersonCode { get; set; }

        public string[] UserGenders { get; set; }
        public string[] UserCitizenships { get; set; }

        public DateTimeOffset? CredentialGrantedSince { get; set; }
        public DateTimeOffset? CredentialGrantedBefore { get; set; }

        public DateTimeOffset? EnrollmentCreatedSince { get; set; }
        public DateTimeOffset? EnrollmentCreatedBefore { get; set; }

        public bool? IsLearner { get; set; }
        public bool? IsAdministrator { get; set; }

        public VLearnerActivityFilter()
        {

        }

        public VLearnerActivityFilter Clone()
        {
            return (VLearnerActivityFilter)MemberwiseClone();
        }
    }
}
