using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptPinCriteria
    {
        QueryFilter Filter { get; set; }
        
        string OptionText { get; set; }

        int? OptionKey { get; set; }
        int? OptionSequence { get; set; }
        int? PinX { get; set; }
        int? PinY { get; set; }
        int? QuestionSequence { get; set; }

        decimal? OptionPoints { get; set; }
    }
}