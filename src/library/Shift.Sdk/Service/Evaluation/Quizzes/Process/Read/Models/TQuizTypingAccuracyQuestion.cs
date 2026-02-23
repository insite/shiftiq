using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Quizzes.Read
{
    [Serializable]
    public class TQuizTypingAccuracyQuestion
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<TQuizTypingAccuracyColumn> Columns { get; set; } = new List<TQuizTypingAccuracyColumn>();

        private bool ShouldSerializeColumns() => Columns.IsNotEmpty();

        public TQuizTypingAccuracyQuestion Clone()
        {
            return new TQuizTypingAccuracyQuestion
            {
                Columns = Columns.Select(x => x.Clone()).ToList()
            };
        }
    }
}
