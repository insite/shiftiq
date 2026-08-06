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

        /// <summary>
        /// Restricts the result to programs that can legally become a parent of the given program:
        /// not the program itself, not already one of its parents, and not a program that has
        /// parents of its own, because nesting is limited to one level. Pass Guid.Empty for a
        /// program that does not exist yet, where only the nesting rule applies.
        /// </summary>
        public Guid? EligibleParentForProgramIdentifier { get; set; }

        public TProgramFilter Clone()
        {
            return (TProgramFilter)MemberwiseClone();
        }
    }
}
