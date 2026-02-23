using System;
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ResultAnalysisFilter : Filter
    {
        #region Classes

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class AnswerFilterItem
        {
            #region Properties

            [JsonProperty(PropertyName = "question")]
            public int QuestionId { get; set; }

            [JsonProperty(PropertyName = "comparison")]
            public ComparisonType ComparisonType { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string AnswerText { get; set; }

            #endregion

            #region Construction

            private AnswerFilterItem()
            {

            }

            public AnswerFilterItem(int questionId, ComparisonType comparison, string answerText)
            {
                QuestionId = questionId;
                ComparisonType = comparison;
                AnswerText = answerText;
            }

            #endregion
        }

        #endregion

        #region Properties

        public int? AssessmentStatusId { get; set; }
        public int? AssetId { get; set; }
        public int? SurveyId { get; set; }

        [JsonProperty(PropertyName = "selections", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(true)]
        public bool ShowSelections { get; set; }

        [JsonProperty(PropertyName = "nums", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(true)]
        public bool ShowNumbers { get; set; }

        [JsonProperty(PropertyName = "comments", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(true)]
        public bool ShowComments { get; set; }

        [JsonProperty(PropertyName = "text", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(true)]
        public bool ShowText { get; set; }

        [JsonProperty(PropertyName = "enableFilter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool EnableQuestionFilter { get; set; }

        [JsonProperty(PropertyName = "enableSubmitted", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool EnableResultFilter { get; set; }

        [JsonProperty(PropertyName = "filter")]
        public List<AnswerFilterItem> AnswerFilter { get; private set; }

        [JsonProperty(PropertyName = "submittedSince", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? UtcSubmittedSince { get; set; }

        [JsonProperty(PropertyName = "submittedBefore", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? UtcSubmittedBefore { get; set; }

        public string AnswerFilterOperator { get; set; }

        #endregion

        #region Construction

        public ResultAnalysisFilter()
        {
            ShowSelections = true;
            ShowNumbers = true;
            ShowComments = true;
            ShowText = true;
            EnableQuestionFilter = false;
            EnableResultFilter = false;

            AnswerFilter = new List<AnswerFilterItem>();
        }

        #endregion
    }
}
