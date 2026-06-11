using System;

using Shift.Common;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TProgramFilter : Filter
    {
        public Guid? CatalogIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid? EnrollmentUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid[] AchievementIdentifiers { get; set; }
        public Guid[] TaskObjectIdentifiers { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramDescription { get; set; }
        public string ProgramName { get; set; }
        public string ProgramTag { get; set; }

        public TProgramFilter Clone()
        {
            return (TProgramFilter)MemberwiseClone();
        }
    }
}
