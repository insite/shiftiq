using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.QuizAttempts.Read
{
    [Serializable]
    public class TQuizAttemptTypingAccuracyQuestion
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<TQuizAttemptTypingAccuracyColumn> Columns { get; set; } = new List<TQuizAttemptTypingAccuracyColumn>();

        private bool ShouldSerializeColumns() => Columns.IsNotEmpty();
    }
}
