using System;

namespace Shift.Sdk.UI
{
    public sealed class CompetencyPositionParameter
    {
        public Guid CompetencyStandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public Type CriteriaType { get; set; }
        public String SearchRouteAction { get; set; }
        public Boolean IsCompetenciesToValidate { get; set; }
    }
}
