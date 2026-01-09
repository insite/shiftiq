using System.Collections.Generic;

using InSite.Application.Surveys.Read;
using InSite.Domain.Surveys.Forms;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Questions.Controls
{
    public partial class DetailAnswerRepeater : AdminBaseControl
    {
        public void BindModelToControls(SurveyQuestion question)
        {
            var answers = ServiceLocator.SurveySearch.GetResponseAnswers(question.Identifier);
            AnswerRepeater.DataSource = CreateModel(question, answers);
            AnswerRepeater.DataBind();
        }

        private List<DetailAnswerRepeaterItem> CreateModel(SurveyQuestion question, VSurveyResponseAnswer[] answers)
        {
            var isNameVisible = !question.Form.EnableUserConfidentiality
                && !Organization.Toolkits.Surveys.EnableUserConfidentiality;

            var list = new List<DetailAnswerRepeaterItem>();
            foreach (var answer in answers)
            {
                var item = new DetailAnswerRepeaterItem();
                if (answer.ResponseSessionCompleted.HasValue)
                    item.ResponseDate = TimeZones.FormatDateOnly(answer.ResponseSessionCompleted.Value, User.TimeZone);
                else if (answer.ResponseSessionStarted.HasValue)
                    item.ResponseDate = TimeZones.FormatDateOnly(answer.ResponseSessionStarted.Value, User.TimeZone);

                if (isNameVisible)
                {
                    item.RespondentName = answer.UserName;
                    item.RespondentEmail = answer.UserEmail;
                }
                else
                {
                    item.RespondentName = "**********";
                }

                item.AnswerText = answer.AnswerText;
                item.OptionText = answer.OptionText;
                list.Add(item);
            }
            return list;
        }
    }

    public class DetailAnswerRepeaterItem
    {
        public string ResponseDate { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string AnswerText { get; set; }
        public string OptionText { get; set; }
    }
}