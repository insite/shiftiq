using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class ScaleItemSerialized
    {
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public string Grade { get; set; }
        public string Calculation { get; set; }
        public List<SurveyContentSerialized> Content { get; set; }
    }
}