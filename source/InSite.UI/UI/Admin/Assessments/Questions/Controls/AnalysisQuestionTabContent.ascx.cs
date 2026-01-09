using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Attempts.Controls;
using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class AnalysisQuestionTabContent : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AnalysisRepeater.ItemDataBound += AnalysisRepeater_ItemDataBound;
        }

        private void AnalysisRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = e.Item;
            var dataItem = item.DataItem;

            var outputTable = (QuestionAnalysisTable)item.FindControl("AnalysisTable");

            var question = (Question)DataBinder.Eval(dataItem, "Question");
            var analysis = (IAttemptAnalysisQuestion[])DataBinder.Eval(dataItem, "AnalysisData");
            var questionAnalysis = analysis?.FirstOrDefault(x => x.Question.Identifier == question.Identifier);

            if (questionAnalysis != null)
                outputTable.SetInputValues(questionAnalysis);
            else
                outputTable.SetInputValues(question);
        }

        public bool LoadData(Question question, InclusionType pilotAttemptsInclusion)
        {
            QuestionRepeater.LoadData(new[] { question });

            var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch)
            {
                Filter = new QAttemptFilter
                {
                    BankIdentifier = question.Set.Bank.Identifier,
                    QuestionIdentifier = question.Identifier,
                    PilotAttemptsInclusion = pilotAttemptsInclusion
                }
            };

            var analysis = AttemptAnalysis.Create(settings);

            if (analysis.HasData)
            {
                var forms = analysis.Forms.Values.OrderBy(x => x.Name).ToArray();
                var range = forms.Length == 1
                    ? Enumerable.Range(0, forms.Length)
                    : Enumerable.Range(-1, forms.Length + 1);

                AnalysisRepeater.DataSource = range.Select((int index) =>
                {
                    var formName = "All Forms";
                    var currentAnalysis = analysis;

                    if (index >= 0)
                    {
                        var form = forms[index];

                        formName = form.Name;
                        currentAnalysis = analysis.FilterAttempt(x => x.FormIdentifier == form.Identifier);
                    }

                    return new
                    {
                        FormName = formName,
                        AnalysisData = currentAnalysis.GetQuestionAnalysis(),
                        Question = question
                    };
                });
                AnalysisRepeater.DataBind();
            }
            else
            {
                MessageLiteral.Text = $"<h3>Answers</h3><p>This question has no completed exam attempts.</p>";
            }

            return analysis.HasData;
        }
    }
}