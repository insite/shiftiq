using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Surveys.Sessions
{
    /// <summary>
    /// A survey response answer contains the text input by the respondent and/or the option items selected by the 
    /// respondent.
    /// </summary>
    [Serializable]
    public class ResponseAnswer
    {
        [JsonIgnore]
        public ResponseSession Session { get; set; }

        /// <summary>
        /// The question answered by the respondent.
        /// </summary>
        public Guid Question { get; set; }

        /// <summary>
        /// The answer input by the respondent.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// The options available for selection. Note this list is ordered in the sequence displayed to the respondent.
        /// </summary>
        public List<ResponseOption> Options { get; set; }

        #region Construction

        public ResponseAnswer()
        {
            Options = new List<ResponseOption>();
        }

        public ResponseAnswer(Guid question) : this()
        {
            Question = question;
        }

        #endregion
    }
}
