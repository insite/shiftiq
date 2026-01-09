using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectAttemptPins : Query<IEnumerable<AttemptPinModel>>, IAttemptPinCriteria
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