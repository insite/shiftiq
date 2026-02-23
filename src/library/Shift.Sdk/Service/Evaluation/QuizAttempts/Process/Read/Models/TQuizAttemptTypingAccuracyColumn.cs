using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.QuizAttempts.Read
{
    [Serializable]
    public class TQuizAttemptTypingAccuracyColumn
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<TQuizAttemptTypingAccuracyRow> Rows { get; set; } = new List<TQuizAttemptTypingAccuracyRow>();

        private bool ShouldSerializeRows() => Rows.IsNotEmpty();
    }
}
