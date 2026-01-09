using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class VProgramEnrollmentFilter : Filter
    {
        public Guid? ProgramIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public Guid? UserIdentifier
        {
            get => UserIdentifiers != null && UserIdentifiers.Length == 1 ? UserIdentifiers[0] : (Guid?)null;
            set => UserIdentifiers = value.HasValue ? new[] { value.Value } : null;
        }
        public Guid[] UserIdentifiers { get; set; }

        public string ProgramName { get; set; }
        public string UserFullName { get; set; }

        public bool CompletionDateIsEmpty { get; set; }

        public VProgramEnrollmentFilter Clone()
        {
            return (VProgramEnrollmentFilter)MemberwiseClone();
        }
    }
}
