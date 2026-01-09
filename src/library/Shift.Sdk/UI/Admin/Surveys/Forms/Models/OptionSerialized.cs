using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class OptionSerialized
    {
        public string Category { get; set; }

        public List<OptionItemSerialized> Items { get; set; }
        public List<SurveyContentSerialized> Content { get; set; }
    }
}