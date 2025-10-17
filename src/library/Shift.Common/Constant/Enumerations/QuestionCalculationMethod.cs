using System.ComponentModel;

namespace Shift.Constant
{
    public enum QuestionCalculationMethod
    {
        [Description("Default")]
        Default,

        [Description("All or Nothing")]
        AllOrNothing,

        [Description("Equally Weighted")]
        EquallyWeighted,

        [Description("Correct Minus Incorrect")]
        CorrectMinusIncorrect,

        [Description("Limited Correct")]
        LimitedCorrect
    }
}