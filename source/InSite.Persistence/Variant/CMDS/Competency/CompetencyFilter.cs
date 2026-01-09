using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    [Serializable]
    public class CompetencyFilter : Filter
    {
        public String Number { get; set; }
        public String Summary { get; set; }
        public String NumberOld { get; set; }
        public Guid? CategoryIdentifier { get; set; }
        public String Description { get; set; }
        public Boolean IsDeleted { get; set; }
        public Guid[] Profiles { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid[] ExcludeCompetencies { get; set; }

        public CompetencyFilter Clone()
        {
            return (CompetencyFilter)MemberwiseClone();
        }
    }
}
