using System;

namespace Shift.Sdk.UI
{
    public class DepartmentCompetency
    {
        public Guid ProfileStandardIdentifier { get; set; }
        public Guid CompetencyStandardIdentifier { get; set; }
        public Guid DepartmentIdentifier { get; set; }
        public Boolean IsSelected { get; set; }
        public Boolean IsTimeSensitive { get; set; }
        public Int32? ValidForCount { get; set; }
        public String ValidForUnit { get; set; }
        public Boolean IsCritical { get; set; }
    }
}