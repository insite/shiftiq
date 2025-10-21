using System;

namespace InSite.Persistence
{
    public class DepartmentProfileCompetency
    {
        public Boolean IsCritical { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Int32? LifetimeMonths { get; set; }
        public Guid ProfileStandardIdentifier { get; set; }

        public virtual Standard Competency {get;set;}
        public virtual Standard Profile { get; set; }
        public virtual Department Department { get; set; }
    }
}
