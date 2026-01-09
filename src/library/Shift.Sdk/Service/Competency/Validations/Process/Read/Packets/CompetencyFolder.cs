using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Standards.Read
{
    [Serializable]
    public class CompetencyArea
    {
        public Guid Identifier { get; set; }
        public string Label { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public List<CompetencyItem> Items { get; set; }
        
        [JsonIgnore]
        public decimal Score
        {
            get
            {
                var questionPoints = GetQuestionsPoints();
                return questionPoints == 0 ? 0 : Calculator.GetPercentDecimal(GetAnswersPoints(), questionPoints);
            }
        }

        [JsonIgnore]
        public string ScoreHtml
        {
            get
            {
                var color = Score > 0.75m ? "success" : (Score > 0.5m ? "warning" : "danger");
                return $"<span class='text-{color}'>{Score:p0}</span>";
            }
        }

        public CompetencyArea()
        {
            Items = new List<CompetencyItem>();
        }

        public int GetQuestionsCount() => Items.Sum(x => x.Questions.Count);

        public decimal GetAnswersPoints() => Items.Sum(x => x.AnswersPoints);

        public decimal GetQuestionsPoints() => Items.Sum(x => x.QuestionsPoints);
    }
}
