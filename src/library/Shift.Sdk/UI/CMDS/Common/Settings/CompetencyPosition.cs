using System;

namespace Shift.Sdk.UI
{
    public sealed class CompetencyPosition
    {
        public Int32 Count { get; set; }
        public Int32 CurrentNumber { get; set; }
        public Guid NextCompetencyStandardIdentifier { get; set; }
        public Guid PrevCompetencyStandardIdentifier { get; set; }
    }
}
