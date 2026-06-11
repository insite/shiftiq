using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardValidationFilter : Filter
    {
        public Guid? DepartmentIdentifier { get; set; }
        public Guid? StandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? ValidatorUserIdentifier { get; set; }

        public string SelfAssessmentStatus { get; set; }
        public string StandardType { get; set; }
        public string ValidationStatus { get; set; }
    }
}