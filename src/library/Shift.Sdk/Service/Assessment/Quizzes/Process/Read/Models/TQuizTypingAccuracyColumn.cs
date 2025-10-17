using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Quizzes.Read
{
    [Serializable]
    public class TQuizTypingAccuracyColumn
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<TQuizTypingAccuracyRow> Rows { get; set; } = new List<TQuizTypingAccuracyRow>();

        private bool ShouldSerializeRows() => Rows.IsNotEmpty();

        public TQuizTypingAccuracyColumn Clone()
        {
            return new TQuizTypingAccuracyColumn
            {
                Rows = Rows.Select(x => x.Clone()).ToList()
            };
        }
    }
}
