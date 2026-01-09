using System;

using Shift.Common;

namespace InSite.Persistence
{
    public class PersonExamRegistrationFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid? EmployerGroupIdentifier { get; set; }
        public Guid[] SavedIdentifiers { get; set; }
        public string PersonCode { get; set; }
        public string PersonName { get; set; }
        public string PersonEmail { get; set; }
    }
}
