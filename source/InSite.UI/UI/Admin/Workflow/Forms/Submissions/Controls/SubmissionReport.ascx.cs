using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Domain.Surveys.Forms;
using InSite.Domain.Surveys.Sessions;
using InSite.UI.Portal.Workflow.Forms.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Submissions.Controls
{
    public partial class SubmissionReport : UserControl
    {
        protected SubmissionSessionState Current { get; set; }
        protected SubmissionSessionNavigator Navigator { get; set; }

        private void AnswerGroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var group = (AnswerGroup)e.Item.DataItem;
                var itemRepeater = (Repeater)e.Item.FindControl("AnswerItemRepeater");
                itemRepeater.ItemDataBound += AnswerItemRepeater_ItemDataBound;
                itemRepeater.DataSource = group.AnswerPackets;
                itemRepeater.DataBind();
            }
        }

        private void AnswerItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (AnswerItem)e.Item.DataItem;
                var ah = (Literal)e.Item.FindControl("AnswerHtml");
                ah.Text = CreateAnswerHtml(item);
            }
        }

        private string CreateAnswerHtml(AnswerItem item)
        {
            var html = new StringBuilder();

            if (item.AnswerInputType == SurveyQuestionType.BreakQuestion)
                return html.ToString();

            html.Append("<div class=\"row\"><div class='col-md-12'><ul>");

            foreach (var option in item.AnswerOptions)
                html.Append($"<li>{option}</li>");

            if (item.AnswerText.HasValue())
            {
                if (item.AnswerInputType == SurveyQuestionType.Upload)
                {
                    var urls = StringHelper.Split(item.AnswerText);
                    foreach (var url in urls)
                    {
                        var href = ServiceLocator.Urls.GetApplicationUrl(Current.Identity.Organization.Code) + url;
                        string name = System.IO.Path.GetFileName(url);
                        html.Append($"<li><a href='{href}'>{name}</a></li>");
                    }
                }
                else
                {
                    var answerTextHtml = !string.IsNullOrEmpty(item.AnswerText)
                        ? item.AnswerText.Replace("\n", "<br/>")
                        : item.AnswerText;

                    html.Append($"<li>{answerTextHtml}</li>");
                }
            }
            else if (item.AnswerOptions.Length == 0 && item.LikertScales.Length == 0)
            {
                html.Append("<li>No Submission</li>");
            }

            html.Append("</ul>");

            AppendLikertScales(html, item);

            html.Append("</div></div>");

            return html.ToString();
        }

        private static void AppendLikertScales(StringBuilder html, AnswerItem answer)
        {
            if (answer.LikertScales.Length == 0)
                return;

            html.AppendLine("<div>");

            foreach (var item in answer.LikertScales)
                html.AppendLine(item);

            if (answer.LikertScaleHighestPoints.HasValue)
            {
                html.AppendLine("<div>");
                html.AppendLine($"<b>{answer.LikertScaleHighestPoints:n2} Points (Highest)</b>");
                html.AppendLine("</div>");
            }

            html.AppendLine("</div>");
        }

        public void LoadReport(SubmissionSessionState current, SubmissionSessionNavigator navigator)
        {
            Current = current;
            Navigator = navigator;

            BindAnswers();
        }

        private void BindAnswers()
        {
            FormTitle.InnerHtml = $"{Current.Survey.Name} <span class='form-text'>Form #{Current.Survey.Asset}</span>";

            RespondentRow.Visible = !Current.Survey.EnableUserConfidentiality;
            RespondentFullName.Text = string.Format("<a href=\"/ui/admin/contacts/people/edit?contact={0}\">{1}</a>", Current.Respondent.UserIdentifier, Current.Respondent.FullName);
            RespondentEmail.Text = string.Format("<a href='mailto:{0}'>{0}</a>", Current.Respondent.Email);

            var questions = Current.Survey.Questions.Where(x => x.HasInput || x.Type == SurveyQuestionType.BreakQuestion).ToList();
            var answerGroups = GetAnswers(questions);

            AnswerGroupRepeater.ItemDataBound += AnswerGroupRepeater_ItemDataBound;
            AnswerGroupRepeater.DataSource = answerGroups;
            AnswerGroupRepeater.DataBind();
        }

        private AnswerGroup[] GetAnswers(List<SurveyQuestion> questions)
        {
            var answers = questions.Select(
                x => new AnswerItem
                {
                    Question = x.Identifier,
                    QuestionSequence = Navigator.GetQuestionNumber(x.Identifier),
                    QuestionBody = x.Content?.Title.GetHtml(),
                    QuestionIsNested = x.IsNested,
                    QuestionIsHidden = !string.IsNullOrEmpty(x.LikertAnalysis),
                    AnswerInputType = x.Type,
                }
                ).ToList();

            foreach (var answer in answers)
                SetAnswerItem(answer);

            var groups = CreateAnswerGroups(answers);

            return groups;
        }

        private void SetAnswerItem(AnswerItem answer)
        {
            var question = Current.Survey.FindQuestion(answer.Question);
            if (question != null)
            {
                if (question.Code.HasValue())
                {
                    var indicator = question.GetIndicatorStyleName();
                    answer.QuestionHeader = $"Question <span class='badge bg-{indicator}'>{question.Code}</span>";
                }
                else if (answer.AnswerInputType == SurveyQuestionType.BreakQuestion)
                {
                    answer.QuestionHeader = $"{answer.QuestionBody}";
                    answer.QuestionBody = string.Empty;
                }
                else if (answer.QuestionSequence > 0)
                {
                    answer.QuestionHeader = $"Question <span class='badge bg-primary'>{answer.QuestionSequence}</span>";
                }
                else
                {
                    answer.IsQuestionHeaderVisible = false;
                }
                answer.AnswerText = GetAnswerText(Current.Session, question.Identifier);
                answer.AnswerOptions = GetAnswerOptions(Current.Session, question.Identifier);

                (answer.LikertScales, answer.LikertScaleHighestPoints) = GetLikertScales(Current.Session, question);
            }
        }

        private AnswerGroup[] CreateAnswerGroups(List<AnswerItem> answers)
        {
            var groups = new List<AnswerGroup>();

            for (int i = 0; i < answers.Count; i++)
            {
                var packet = answers[i];

                if (i == 0)
                {
                    groups.Add(new AnswerGroup(packet));
                }
                else if (packet.QuestionIsNested)
                {
                    var nest = groups.Last();
                    nest.AnswerPackets.Add(packet);
                }
                else
                {
                    groups.Add(new AnswerGroup(packet));
                }
            }

            return groups.ToArray();
        }

        private string GetAnswerText(QResponseSession response, Guid question)
        {
            return response.QResponseAnswers
                .Where(x => x.SurveyQuestionIdentifier == question)
                .Select(x => x.ResponseAnswerText)
                .FirstOrDefault();
        }

        private string[] GetAnswerOptions(QResponseSession response, Guid question)
        {
            var options = response.QResponseOptions.Where(x => x.SurveyQuestionIdentifier == question).ToArray();
            if (options.IsEmpty())
                return new string[0];

            var list = new List<string>();

            foreach (var option in options)
            {
                if (!option.ResponseOptionIsSelected)
                    continue;

                var item = Current.Survey.FindOptionItem(option.SurveyOptionIdentifier);
                if (item != null)
                {
                    var listTitle = item.List.Content?.Title.GetText();
                    var itemTitle = item.Content.Title.GetText();
                    list.Add($"<strong class='me-3'>{itemTitle}</strong> {listTitle}");
                }
            }

            return list.ToArray();
        }

        private (string[], decimal?) GetLikertScales(QResponseSession response, SurveyQuestion question)
        {
            if (question.Type != SurveyQuestionType.Likert)
                return (new string[0], null);

            var options = response.QResponseOptions
                .Where(x => x.SurveyQuestionIdentifier == question.Identifier)
                .OrderBy(x => x.OptionSequence)
                .ToArray();

            var likertType = ReviewDetails.GetLikertType(question.LikertAnalysis);

            var likertScale = ReviewDetails.GetLikertScale(question, options, response.QResponseOptions, likertType);
            if (likertScale?.Items == null || likertScale.Items.Count == 0)
                return (new string[0], null);

            var result = new List<string>();

            foreach (var item in likertScale.Items)
            {
                var itemText = new StringBuilder();
                itemText.AppendLine("<div>");
                itemText.AppendLine("<h2>");
                itemText.AppendLine(item.Category);
                itemText.AppendLine("</h2>");
                itemText.AppendLine("<h3>");
                itemText.AppendLine(item.Grade);

                if (!string.IsNullOrWhiteSpace(item.Calculation))
                {
                    var value = string.Equals(item.Calculation, "Average", StringComparison.OrdinalIgnoreCase)
                        ? item.Average
                        : item.Points;

                    itemText.AppendLine($"{value:n2}");
                }

                itemText.AppendLine("</h3>");

                itemText.AppendLine(item.FeedbackHtml);
                itemText.AppendLine("</div>");

                result.Add(itemText.ToString());
            }

            return (result.ToArray(), likertScale.HighestPoints);
        }
    }
}