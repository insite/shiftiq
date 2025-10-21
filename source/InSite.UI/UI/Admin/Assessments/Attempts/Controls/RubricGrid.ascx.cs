using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class RubricGrid : BaseUserControl
    {
        class AnswerItem
        {
            public int QuestionSequence { get; set; }
            public string QuestionText { get; set; }
            public string RubricTitle { get; set; }
            public decimal? QuestionPoints { get; set; }
            public decimal? AnswerPoints { get; set; }
        }

        public void LoadData(QAttemptQuestion[] questions)
        {
            var lang = Identity.Language;
            var answers = new List<AnswerItem>();
            var rubrics = ServiceLocator.RubricSearch.GetQuestionRubrics(questions.Select(x => x.QuestionIdentifier));
            var contents = lang != Language.Default && rubrics.Count > 0
                ? ServiceLocator.ContentSearch.GetBlocks(rubrics.Values.Select(x => x.RubricIdentifier).Distinct(), lang, new[] { ContentLabel.Title })
                : null;

            foreach (var q in questions)
            {
                var rubric = rubrics.GetOrDefault(q.QuestionIdentifier);
                var rubricContent = rubric != null ? contents?.GetOrDefault(rubric.RubricIdentifier) : null;

                var answer = new AnswerItem
                {
                    QuestionSequence = q.QuestionSequence,
                    QuestionText = q.QuestionText,
                    RubricTitle = (rubricContent?.Title.GetText(lang)).IfNullOrEmpty(rubric?.RubricTitle),
                    QuestionPoints = q.QuestionPoints,
                    AnswerPoints = q.AnswerPoints
                };

                answers.Add(answer);
            }

            Rubrics.DataSource = answers;
            Rubrics.DataBind();
        }

        protected string FormatScore()
        {
            var answer = (AnswerItem)Page.GetDataItem();

            return answer.AnswerPoints == null
                ? $"<span class='badge bg-warning'>{Translate("Not Graded")}</span>"
                : $"<div>{answer.AnswerPoints} / {answer.QuestionPoints}</div> <span class='badge bg-success'>{Translate("Graded")}</span>";
        }
    }
}