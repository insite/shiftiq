using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAttemptPins : Query<int>, IAttemptPinCriteria
    {
        public string OptionText { get; set; }

        public int? OptionKey { get; set; }
        public int? OptionSequence { get; set; }
        public int? PinX { get; set; }
        public int? PinY { get; set; }
        public int? QuestionSequence { get; set; }

        public decimal? OptionPoints { get; set; }
    }
}