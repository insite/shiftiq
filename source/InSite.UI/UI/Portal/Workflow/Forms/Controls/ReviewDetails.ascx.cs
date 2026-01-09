using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class ReviewDetails : BaseUserControl
    {
        #region Classes

        protected class CardInfo
        {
            public string Title { get; set; }

            public List<QuestionInfo> Questions { get; set; } = new List<QuestionInfo>();
        }

        protected class QuestionInfo
        {
            public Guid Identifier { get; set; }
            public SurveyQuestionType Type { get; set; }
            public string Html { get; set; }
            public bool IsHidden { get; set; }

            public AnswerInfo Answer { get; set; }
        }

        protected class AnswerInfo
        {
            public string Text { get; set; }

            public LikertScaleInfo LikertScale { get; internal set; }
            public List<AnswerOptionInfo> Options { get; } = new List<AnswerOptionInfo>();
        }

        protected class AnswerOptionInfo
        {
            public string Header { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Feedback { get; set; }
        }

        public enum LikertAnalysisType
        {
            Default, Strategy1, Strategy2
        }

        public class LikertScaleInfo
        {
            public decimal? HighestPoints { get; set; }

            public List<LikertScaleItemInfo> Items { get; set; }

            public LikertScaleInfo()
            {
                Items = new List<LikertScaleItemInfo>();
            }
        }

        public class LikertScaleItemInfo
        {
            public string Category { get; set; }
            public string Grade { get; set; }
            public string Calculation { get; set; }
            public string FeedbackHtml { get; set; }
            public decimal Points { get; set; }
            public int Count { get; set; }
            public decimal Average
            {
                get
                {
                    if (Count == 0)
                        return 0;
                    return Points / Count;
                }
            }

            public LikertScaleItemInfo(SurveyScale scale, SurveyScaleItem scaleItem, decimal points, int count)
            {
                Category = scale.Category;
                Grade = scaleItem.Grade;
                Calculation = scaleItem.Calculation;
                FeedbackHtml = FillPlaceholders(scaleItem.Content.Description.GetHtml());
                Points = points;
                Count = count;
            }

            private string FillPlaceholders(string html)
            {
                if (html != null && User != null)
                    html = html.Replace("$UserIdentifier", User.UserIdentifier.ToString());
                return html;
            }
        }

        private class DataSource
        {
            public IEnumerable<CardInfo> Cards { get; set; }
            public IEnumerable<QuestionInfo> BarChartQuestions { get; set; }
        }

        #endregion

        #region Fields

        private SubmissionSessionState _state;
        private SubmissionSessionNavigator _navigator;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CardRepeater.ItemCreated += CardRepeater_ItemCreated;
            CardRepeater.ItemDataBound += CardRepeater_ItemDataBound;

            CommonStyle.ContentKey = typeof(ReviewDetails).FullName;
        }

        #endregion

        #region Event handlers

        private void CardRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var questionRepeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            questionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
        }

        private void CardRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var card = (CardInfo)e.Item.DataItem;
            var questionRepeater = (Repeater)e.Item.FindControl("QuestionRepeater");
            questionRepeater.DataSource = card.Questions;
            questionRepeater.DataBind();
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var question = (QuestionInfo)e.Item.DataItem;

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = question.Answer?.Options;
            optionRepeater.DataBind();

            var scaleRepeater = (Repeater)e.Item.FindControl("ScaleRepeater");
            scaleRepeater.ItemDataBound += ScaleRepeater_ItemDataBound;
            scaleRepeater.DataSource = question.Answer?.LikertScale?.Items;
            scaleRepeater.DataBind();
            scaleRepeater.Visible = true;
        }

        private void ScaleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (LikertScaleItemInfo)e.Item.DataItem;
            if (string.IsNullOrWhiteSpace(item.Calculation))
                return;

            var value = string.Equals(item.Calculation, "Average", StringComparison.OrdinalIgnoreCase)
                ? item.Average
                : item.Points;

            var gradeValue = (ITextControl)e.Item.FindControl("GradeValue");
            gradeValue.Text = $"{value:n2}";
        }

        #endregion

        #region Data binding

        internal void LoadData(SubmissionSessionState state, SubmissionSessionNavigator navigator, UserFeedbackType feedbackType, bool print)
        {
            _state = state;
            _navigator = navigator;

            BindCompletedInstructions();

            DataSource dataSource;

            if (feedbackType == UserFeedbackType.Summary)
            {
                dataSource = GetDataSource(false);
                dataSource.Cards = dataSource.Cards.ForEach(card =>
                {
                    card.Questions = card.Questions
                        .Where(
                            q => q.Answer != null
                            && (
                                q.Answer.Options.Any(o => o.Description.IsNotEmpty() || o.Feedback.IsNotEmpty())
                                ||
                                (q.Answer.LikertScale?.Items != null && q.Answer.LikertScale.Items.Any())
                            )
                        )
                        .ToList();
                }).Where(x => x.Questions.Any());
            }
            else if (feedbackType == UserFeedbackType.Disabled)
            {
                dataSource = new DataSource();
            }
            else
            {
                dataSource = GetDataSource(feedbackType == UserFeedbackType.Answered);
            }

            BarChartPanel.Visible = dataSource.BarChartQuestions != null;

            if (dataSource.BarChartQuestions != null)
                BuildSummaryBarChart(dataSource.BarChartQuestions, print);

            CardRepeater.DataSource = dataSource.Cards;
            CardRepeater.DataBind();
        }

        private void BindCompletedInstructions()
        {
            var text = _state.Survey.Content?[ContentLabel.CompletedInstructions]?.GetHtml(_state.Language);

            CompletedInstructionsPanel.Visible = !string.IsNullOrEmpty(text);

            CompletedInstructionsLiteral.Text = text;
        }

        private void BuildSummaryBarChart(IEnumerable<QuestionInfo> questions, bool print)
        {
            var firstQuestion = questions.FirstOrDefault();

            var categories = firstQuestion
                .Answer.LikertScale.Items.Select(x => x.Category)
                .ToList();

            var legend = new List<Tuple<string, string>>();
            var colorsText = new StringBuilder();
            var seriesText = new StringBuilder();

            for (int i = 0; i < categories.Count; i++)
            {
                var color = GetBarChartSeriesColor(i);
                var reverseColor = GetBarChartSeriesColor(categories.Count - i - 1);

                legend.Add(new Tuple<string, string>(categories[i], reverseColor));

                if (colorsText.Length > 0)
                    colorsText.Append(',');

                colorsText.Append($@"""{color}""");

                if (seriesText.Length > 0)
                    seriesText.Append(",");

                seriesText.Append('[');

                var first = true;

                foreach (var question in questions)
                {
                    if (!first)
                        seriesText.Append(',');
                    else
                        first = false;

                    var items = question.Answer.LikertScale.Items;

                    if (i < items.Count)
                        seriesText.Append(items[i].Average.ToString());
                }

                seriesText.Append(']');
            }

            var height = Number.CheckRange(70 + 10 * categories.Count, 100);
            var labelsList = questions.Select(x => StringHelper.StripHtml(x.Html?.Replace("\"", "")?.Replace("\n", " ")));
            var labelsText = string.Join(",", labelsList.Select(x => $"\"{x}\""));

            var html = new StringBuilder();
            html.Append($@"<div class=""ct-chart"" style=""height:{height}px;""");
            html.Append($@" data-bar-chart='{{""labels"": [{labelsText}], ""series"": [{seriesText}] }}'");
            html.Append($@" data-series-color='{{""colors"": [{colorsText}]}}'");

            var offset = labelsList.Any(x => !string.IsNullOrWhiteSpace(x))
                ? print ? 350 : 500
                : 0;

            if (print)
                html.Append(@" data-options='{ ""horizontalBars"": true, ""reverseData"": true, ""seriesBarDistance"": 10, ""axisY"": { ""offset"": " + offset + @" } , ""width"": 750, ""height"": " + height + "}'");
            else
                html.Append(@" data-options='{ ""horizontalBars"": true, ""reverseData"": true, ""seriesBarDistance"": 10, ""axisY"": { ""offset"": " + offset + @" } }'");

            html.Append("></div>");

            ChartLegend.DataSource = legend;
            ChartLegend.DataBind();

            Chart.Text = html.ToString();
        }

        private static string GetBarChartSeriesColor(int index)
        {
            var colors = new[] { "#4472C4", "#ED7D31", "#F4C63D", "#453D3F", "#59922B" };
            return colors[index % 5];
        }

        #endregion

        #region Methods (answers)

        private DataSource GetDataSource(bool onlyAnswered)
        {
            var cards = new List<CardInfo>();
            var barChartQuestions = new List<QuestionInfo>();
            CardInfo card = null;

            foreach (var q in _state.Survey.Questions)
            {
                if (!q.HasInput && q.Type != SurveyQuestionType.BreakQuestion)
                    continue;

                if (card == null || q.Type == SurveyQuestionType.BreakQuestion || !q.IsNested)
                {
                    cards.Add(card = new CardInfo());

                    if (q.Code.HasValue())
                    {
                        var indicator = q.GetIndicatorStyleName();
                        card.Title = $"{Global.Translate("Question")} <span class='fs-2 badge bg-{indicator}'>{q.Code}</span>";
                    }
                    else if (q.Type == SurveyQuestionType.BreakQuestion)
                    {
                        card.Title = $"{q.Content?.Title.GetHtml(_state.Language, true)}";
                    }
                    else
                    {
                        var sequence = _navigator.GetQuestionNumber(q.Identifier);
                        if (sequence > 0)
                            card.Title = $"{Global.Translate("Question")} {sequence.ToString()}";
                    }
                }

                if (q.Type == SurveyQuestionType.BreakQuestion)
                    continue;

                var answer = _state.Session.QResponseAnswers.FirstOrDefault(x => x.SurveyQuestionIdentifier == q.Identifier);
                var answerInfo = GetAnswer(q, answer);

                if (onlyAnswered && !HasResponse(answerInfo))
                    continue;

                var question = new QuestionInfo
                {
                    Identifier = q.Identifier,
                    Type = q.Type,
                    Html = q.Content?.Title.GetHtml(_state.Language, true),
                    IsHidden = false
                };

                card.Questions.Add(question);

                if (answerInfo != null)
                {
                    question.Answer = answerInfo;

                    var likertType = GetLikertType(q.LikertAnalysis);
                    if (_state.Survey.DisplaySummaryChart && (likertType == LikertAnalysisType.Strategy1 || likertType == LikertAnalysisType.Strategy2))
                        barChartQuestions.Add(question);
                }
            }

            var nonEmptyCards = onlyAnswered
                ? cards.Where(x => x.Questions.Count > 0).ToList()
                : cards;

            return new DataSource { Cards = nonEmptyCards, BarChartQuestions = barChartQuestions.Count > 0 ? barChartQuestions : null };
        }

        private AnswerInfo GetAnswer(SurveyQuestion q, QResponseAnswer answer)
        {
            if (answer == null)
                return null;

            var options = _state.Session.QResponseOptions
                .Where(x => x.SurveyQuestionIdentifier == q.Identifier)
                .OrderBy(x => x.OptionSequence)
                .ToArray();

            var likertType = GetLikertType(q.LikertAnalysis);

            var answerInfo = new AnswerInfo
            {
                Text = answer.ResponseAnswerText,
                LikertScale = GetLikertScale(q, options, _state.Session.QResponseOptions, likertType)
            };

            if (options.Length == 0)
                return answerInfo;

            foreach (var qao in options)
            {
                var optionItem = _state.Survey.FindOptionItem(qao.SurveyOptionIdentifier);
                if (optionItem == null)
                    continue;

                var header = optionItem.List.Content?.Title.GetText(_state.Language, true);
                var content = optionItem.Content;
                var title = content?.Title?.GetText(_state.Language, true);
                var description = content?.Description?.GetHtml(_state.Language, true);

                if (qao.ResponseOptionIsSelected)
                {
                    answerInfo.Options.Add(new AnswerOptionInfo
                    {
                        Header = header,
                        Title = title,
                        Description = description,
                        Feedback = content?.Feedback?.GetHtml(_state.Language, true)
                    });
                }
                else
                {
                    var feedback = content?.FeedbackWhenNotSelected?.GetHtml(_state.Language, true);
                    if (!string.IsNullOrWhiteSpace(feedback))
                    {
                        answerInfo.Options.Add(new AnswerOptionInfo
                        {
                            Header = header,
                            Title = title,
                            Description = description,
                            Feedback = content?.FeedbackWhenNotSelected?.GetHtml(_state.Language, true)
                        });
                    }
                }
            }

            return answerInfo;
        }

        #endregion

        #region Methods (likert scales)

        public static LikertScaleInfo GetLikertScale(
            SurveyQuestion question,
            IEnumerable<QResponseOption> options,
            IEnumerable<QResponseOption> allOptions,
            LikertAnalysisType likertType
            )
        {
            if (question.Scales.IsEmpty())
                return null;

            switch (likertType)
            {
                case LikertAnalysisType.Strategy1:
                    return GetLikertScales_Strategy1(question, allOptions);
                case LikertAnalysisType.Strategy2:
                    return GetLikertScales_Strategy2(question, allOptions);
                default:
                    return GetLikertScales_Default(question, options);
            }
        }

        private static LikertScaleInfo GetLikertScales_Default(SurveyQuestion question, IEnumerable<QResponseOption> options)
        {
            if (!options.Any())
                return null;

            var result = new LikertScaleInfo();

            foreach (var scale in question.Scales)
            {
                var points = 0m;
                var count = 0;

                foreach (var option in options)
                {
                    if (!option.ResponseOptionIsSelected)
                        continue;

                    var optionItem = question.Form.FindOptionItem(option.SurveyOptionIdentifier);
                    if (optionItem == null || optionItem.List == null)
                        continue;

                    if (optionItem.List.Category == scale.Category)
                    {
                        points += optionItem.Points;
                        count++;
                    }
                }

                var scaleItem = scale.Items.FirstOrDefault(x => x.Minimum <= points && points <= x.Maximum);
                if (scaleItem != null)
                    result.Items.Add(new LikertScaleItemInfo(scale, scaleItem, points, count));
            }

            return result;
        }

        private static LikertScaleInfo GetLikertScales_Strategy1(SurveyQuestion question, IEnumerable<QResponseOption> allOptions)
        {
            var result = new LikertScaleInfo();

            var questions = question.Form.Questions
                .Where(x => x.Sequence < question.Sequence)
                .Select(x => new { x.Identifier, x.Type })
                .ToList();
            var questionOptions = allOptions
                .GroupBy(x => x.SurveyQuestionIdentifier)
                .ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var scale in question.Scales)
            {
                var points = 0m;
                var count = 0;

                foreach (var index in questions)
                {
                    if (!questionOptions.TryGetValue(index.Identifier, out var options))
                        continue;

                    foreach (var option in options)
                    {
                        if (!option.ResponseOptionIsSelected)
                            continue;

                        var optionItem = question.Form.FindOptionItem(option.SurveyOptionIdentifier);
                        if (optionItem?.List == null)
                            continue;

                        if (optionItem.List.Category == scale.Category && index.Type == SurveyQuestionType.Likert)
                        {
                            points += optionItem.Points;
                            count++;
                        }
                        else if (optionItem.Category == scale.Category && (index.Type == SurveyQuestionType.Selection || index.Type == SurveyQuestionType.RadioList || index.Type == SurveyQuestionType.CheckList))
                        {
                            points += optionItem.Points;
                            count++;
                        }
                    }
                }

                var scaleItem = scale.Items.FirstOrDefault(x => x.Minimum <= points && points <= x.Maximum);
                if (scaleItem != null)
                {
                    if (result.Items == null)
                        result.Items = new List<LikertScaleItemInfo>();
                    result.Items.Add(new LikertScaleItemInfo(scale, scaleItem, points, count));
                }
            }

            return result;
        }

        private static LikertScaleInfo GetLikertScales_Strategy2(SurveyQuestion question, IEnumerable<QResponseOption> allOptions)
        {
            var result = new LikertScaleInfo();

            var previousQuestions = question.Form.Questions
                .Where(x => x.Sequence < question.Sequence)
                .ToList();
            var options = allOptions
                .Where(a => previousQuestions.Any(q => q.Identifier == a.SurveyQuestionIdentifier))
                .ToList();

            var pointsPerScale = new Dictionary<string, decimal>();
            var countsPerScale = new Dictionary<string, int>();

            foreach (var scale in question.Scales)
            {
                decimal points = 0;
                int count = 0;

                foreach (var option in options)
                {
                    if (!option.ResponseOptionIsSelected)
                        continue;

                    var optionItem = question.Form.FindOptionItem(option.SurveyOptionIdentifier);
                    if (optionItem.List.Category == scale.Category)
                    {
                        points += optionItem.Points;
                        count++;
                    }
                }

                pointsPerScale.Add(scale.Category, points);
                countsPerScale.Add(scale.Category, count);
            }

            var highestPoints = pointsPerScale.Values.Max();
            var ok = false;

            foreach (var scale in question.Scales)
            {
                var points = pointsPerScale[scale.Category];
                if (points < highestPoints)
                    continue;

                var count = countsPerScale[scale.Category];
                var scaleItem = scale.Items.FirstOrDefault(x => x.Minimum <= points && points <= x.Maximum);
                if (scaleItem != null)
                {
                    result.Items.Add(new LikertScaleItemInfo(scale, scaleItem, points, count));

                    ok = true;
                }
            }

            if (!ok)
                result.HighestPoints = highestPoints;

            return result;
        }

        #endregion

        #region Methods (helpers)

        public static LikertAnalysisType GetLikertType(string value)
        {
            if (value == "Preceding Questions, All Scales")
                return LikertAnalysisType.Strategy1;

            if (value == "Preceding Questions, Highest-Point Scale Only")
                return LikertAnalysisType.Strategy2;

            return LikertAnalysisType.Default;
        }

        protected string GetUploadLinks()
        {
            var question = (QuestionInfo)Page.GetDataItem();

            if (question.Type != SurveyQuestionType.Upload)
                return string.Empty;

            if (string.IsNullOrEmpty(question.Answer?.Text))
                return string.Empty;

            var links = string.Empty;

            var urls = StringHelper.Split(question.Answer?.Text, ';');
            foreach (var url in urls)
                links += $"<li><a target='_blank' href='{url}' class='mb-3'>{Path.GetFileName(url)}</a></li>";

            return links;
        }

        protected bool HasResponse()
        {
            var question = (QuestionInfo)Page.GetDataItem();
            var answer = question.Answer;

            return answer != null && (
                answer.Options.Count > 0
                || answer.Text.IsNotEmpty()
                || answer.LikertScale != null
                );
        }

        private bool HasResponse(AnswerInfo answer)
        {
            return answer != null && (
                answer.Options.Count > 0
                || answer.Text.IsNotEmpty()
                || answer.LikertScale != null
                );
        }

        protected bool HasUpload()
        {
            var question = (QuestionInfo)Page.GetDataItem();

            return question.Type == SurveyQuestionType.Upload && (question.Answer?.Text).IsNotEmpty();
        }

        protected bool HasComment()
        {
            var question = (QuestionInfo)Page.GetDataItem();

            return question.Type != SurveyQuestionType.Upload && (question.Answer?.Text).IsNotEmpty();
        }

        #endregion
    }
}