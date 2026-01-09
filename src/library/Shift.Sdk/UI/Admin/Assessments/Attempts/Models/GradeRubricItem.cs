using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class GradeRubricItem
    {
        [JsonProperty(PropertyName = "questionId")]
        public Guid QuestionIdentifier { get; set; }

        [JsonProperty(PropertyName = "rubricId")]
        public Guid RubricIdentifier { get; set; }

        public QuestionItemType QuestionType { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public string ExemplarText { get; set; }
        public Guid? AnswerFileIdentifier { get; set; }
        public string RubricTitle { get; set; }
        public decimal RubricPoints { get; set; }

        public decimal SelectedPoints => Criteria.Sum(c => c.SelectedRating.SelectedPoints ?? 0);

        [JsonProperty(PropertyName = "criteria")]
        public List<GradeCriterionItem> Criteria { get; set; }

        public Dictionary<Guid, decimal> InitRatingPoints { get; set; }
    }
}