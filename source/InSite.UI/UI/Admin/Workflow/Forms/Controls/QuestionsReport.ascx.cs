using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Domain.Surveys.Forms;
using InSite.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class QuestionsReport : UserControl
    {
        private class QuestionGroup
        {
            public string QuestionHeader { get; set; }
            public List<QuestionItem> Items { get; set; }
        }

        private class QuestionItem
        {
            public string QuestionBody { get; set; }
        }

        private class PageItem
        {
            public int PageNumber { get; set; }
            public List<QuestionGroup> Groups { get; set; }
        }

        public void LoadReport(SurveyState survey)
        {
            DefaultCss.Text = $@"<link href='{PathHelper.ToAbsoluteUrl("/library/fonts/font-awesome/7.1.0/css/all.min.css")}' rel='stylesheet' type='text/css' media='all'>";

            Title.InnerText = survey != null ? survey.Form.Name : "Invalid Form Key";

            if (survey == null)
            {
                ErrorMessage.InnerText = "Invalid Form Key.";
                ErrorMessage.Visible = true;

                return;
            }

            if (survey.Form.Questions.Count == 0)
            {
                ErrorMessage.InnerText = "The form has no questions.";
                ErrorMessage.Visible = true;
                return;
            }

            var pages = GetPages(survey.Form);

            PageRepeater.ItemDataBound += PageRepeater_ItemDataBound;
            PageRepeater.DataSource = pages;
            PageRepeater.DataBind();
        }

        private void PageRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var page = (PageItem)e.Item.DataItem;

            var questionGroupRepeater = (Repeater)e.Item.FindControl("QuestionGroupRepeater");
            questionGroupRepeater.ItemDataBound += QuestionGroupRepeater_ItemDataBound;
            questionGroupRepeater.DataSource = page.Groups;
            questionGroupRepeater.DataBind();
        }

        private void QuestionGroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var group = (QuestionGroup)e.Item.DataItem;

            var questionItemRepeater = (Repeater)e.Item.FindControl("QuestionItemRepeater");
            questionItemRepeater.DataSource = group.Items;
            questionItemRepeater.DataBind();
        }

        private List<PageItem> GetPages(SurveyForm form)
        {
            var pages = new List<PageItem>();

            foreach (var question in form.Questions)
            {
                if (question.Type == SurveyQuestionType.Terminate)
                    continue;

                PageItem page;

                if (pages.Count == 0 || question.Type == SurveyQuestionType.BreakPage && pages[pages.Count - 1].Groups.Count > 0)
                    pages.Add(page = new PageItem { PageNumber = pages.Count + 1, Groups = new List<QuestionGroup>() });
                else
                    page = pages[pages.Count - 1];

                // Question breaks contain content that is needed in a PDF export of form questions.
                if (question.HasInput || question.Type == SurveyQuestionType.BreakQuestion)
                {
                    QuestionGroup group;

                    if (page.Groups.Count == 0 || !question.IsNested)
                        page.Groups.Add(group = new QuestionGroup { QuestionHeader = GetGroupHeader(question), Items = new List<QuestionItem>() });
                    else
                        group = page.Groups[page.Groups.Count - 1];

                    var item = GetQuestionItem(question);

                    group.Items.Add(item);
                }
            }

            if (pages.Count > 0 && pages[pages.Count - 1].Groups.Count == 0)
                pages.RemoveAt(pages.Count - 1);

            return pages;
        }

        private string GetGroupHeader(SurveyQuestion question)
        {
            var questionHeader = new StringBuilder();
            questionHeader.Append($"<div class=\"header\"><div class=\"title\">");

            var indicator = question.Code.HasValue() ? question.Code : question.Sequence.ToString();
            var indicatorStyle = question.GetIndicatorStyleName();

            questionHeader.Append($"<div class='badge bg-{indicatorStyle}'>{indicator}</div>");
            questionHeader.Append("</div></div>");

            return questionHeader.ToString();
        }

        private QuestionItem GetQuestionItem(SurveyQuestion question)
        {
            var questionBody = new StringBuilder();

            var language = question.Form.Language;
            var questionTitle = question.Content?.Title?.GetHtml(language) ?? "(Question)";

            questionBody.Append($"<div class=\"header\" style='padding-top:0.8em'><div class=\"title\">");

            if (questionTitle.HasValue())
                questionBody.Append(questionTitle);

            questionBody.Append("</div></div>");

            if (question.Type == SurveyQuestionType.Likert)
            {
                GetLikertTableHtml(question, questionBody);
            }
            else
            {
                questionBody.Append("<div class=\"field\" style=\"page-break-inside:avoid;\">");

                switch (question.Type)
                {
                    case SurveyQuestionType.Comment:
                        questionBody.Append("<div class=\"field-comment\"></div>");
                        break;
                    case SurveyQuestionType.Upload:
                        questionBody.Append("<div class=\"field-fileupload\"></div>");
                        break;
                    case SurveyQuestionType.Number:
                        questionBody.Append("<div class=\"field-number\"></div>");
                        break;
                    case SurveyQuestionType.Date:
                        questionBody.Append("<div class=\"field-date\"></div>");
                        break;
                    case SurveyQuestionType.Text:
                        questionBody.Append("<div class=\"field-text\"></div>");
                        break;
                    case SurveyQuestionType.CheckList:
                    case SurveyQuestionType.RadioList:
                    case SurveyQuestionType.Selection:
                        GetListHtml(question, questionBody);
                        break;
                }

                questionBody.Append("</div>");
            }

            return new QuestionItem
            {
                QuestionBody = questionBody.ToString()
            };
        }

        private void GetListHtml(SurveyQuestion question, StringBuilder builder)
        {
            switch (question.Type)
            {
                case SurveyQuestionType.CheckList:
                    builder.Append("<div class=\"field-checklist\">");
                    break;
                case SurveyQuestionType.RadioList:
                    builder.Append("<div class=\"field-radiolist\">");
                    break;
                case SurveyQuestionType.Selection:
                    builder.Append("<div class=\"field-selection\">");
                    break;
            }

            if (!question.Options.IsEmpty)
            {
                var language = question.Form.Language;
                var list = question.Options.Lists[0];

                foreach (var option in list.Items)
                    builder.Append($"<div class=\"option\">{Markdown.ToHtml(option.Content?.Title?.Text[language])}</div>");
            }

            builder.Append("</div>");
        }

        private void GetLikertTableHtml(SurveyQuestion question, StringBuilder builder)
        {
            if (question.Options.IsEmpty)
                return;

            var language = question.Form.Language;
            var firstList = question.Options.Lists[0];

            builder.Append("<div class=\"field\">");
            builder.Append("<table>");

            // Head
            builder.Append("<thead>");
            builder.Append("<tr>");
            builder.Append("<th></th>");

            foreach (var option in firstList.Items)
            {
                builder.Append($"<th>{Markdown.ToHtml(option.Content?.Title?.Text[language])}</th>");
            }

            builder.Append("</tr>");
            builder.Append("</thead>");

            // Body
            builder.Append("<tbody>");

            foreach (var list in question.Options.Lists)
            {
                builder.Append("<tr>");

                builder.Append($"<th>{Markdown.ToHtml(list.Content?.Title?.Text[language])}</th>");

                for (var i = 0; i < list.Items.Count; i++)
                    builder.Append("<td class=\"radio-button\"></td>");

                builder.Append("</tr>");
            }

            builder.Append("</tbody>");

            builder.Append("</table>");
            builder.Append("</div>");
        }
    }
}