using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Quizzes.Read
{
    [Serializable]
    public class TQuizTypingAccuracyRow
    {
        public string Label { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Values { get; set; } = new List<string>();

        private bool ShouldSerializeValues() => Values.IsNotEmpty();

        public TQuizTypingAccuracyRow Clone()
        {
            return new TQuizTypingAccuracyRow
            {
                Label = this.Label,
                Values = Values.ToList()
            };
        }
    }
}
