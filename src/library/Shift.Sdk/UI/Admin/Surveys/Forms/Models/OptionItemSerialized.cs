using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class OptionItemSerialized
    {
        public string Category { get; set; }
        public decimal Points { get; set; }
        public int? BranchToQuestionIndex { get; set; }
        public int[] MaskedQuestionIndexes { get; set; }
        public List<SurveyContentSerialized> Content { get; set; }
    }
}