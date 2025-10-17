using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Standards.Read
{
    [Serializable]
    public class CompetencyItem
    {
        public Guid Identifier { get; set; }
        public string Label { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public List<CompetencyQuestion> Questions { get; set; }

        [JsonIgnore]
        public decimal Score
        {
            get
            {
                var questionPoints = QuestionsPoints;
                return questionPoints == 0 ? 0 : Calculator.GetPercentDecimal(AnswersPoints, questionPoints);
            }
        }

        [JsonIgnore]
        public string ScoreHtml
        {
            get
            {
                var color = Score > 0.75m ? "success" : (Score > 0.50m ? "warning" : "danger");
                return $"<span class='badge bg-{color}'>{Score:p0}</span>";
            }
        }

        [JsonIgnore]
        public decimal QuestionsPoints => Questions.Sum(x => x.QuestionPoints);

        [JsonIgnore]
        public decimal AnswersPoints => Questions.Sum(x => x.AnswerPoints);

        [JsonIgnore]
        public string PointsHtml => $"{AnswersPoints:n2} / {QuestionsPoints:n2}";

        public CompetencyItem()
        {
            Questions = new List<CompetencyQuestion>();
        }
    }
}
