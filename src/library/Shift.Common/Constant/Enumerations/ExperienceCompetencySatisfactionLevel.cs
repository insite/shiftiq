using System.ComponentModel;

namespace Shift.Constant
{
    public enum ExperienceCompetencySatisfactionLevel
    {
        [Description("None")]
        None,

        [Description("Not Satisfied")]
        NotSatisfied,

        [Description("Partially Satisfied")]
        PartiallySatisfied,

        [Description("Satisfied")]
        Satisfied
    };
}