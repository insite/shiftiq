using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VProgramEnrollmentFilter : Filter
    {
        public Guid? ProgramIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string ProgramName { get; set; }
        public string UserFullName { get; set; }

        public bool CompletionDateIsEmpty { get; set; }

        public VProgramEnrollmentFilter Clone()
        {
            return (VProgramEnrollmentFilter)MemberwiseClone();
        }
    }
}
