using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class QuestionAnalysisTable : BaseUserControl
    {
        #region Classes

        private class EmptyAnalysisQuestion : IAttemptAnalysisQuestion
        {
            public Question Question { get; }

            public int AttemptCount => 0;

            public int SuccessCount => 0;

            public double SuccessRate => 0;

            public int NoAnswerCount => 0;

            public decimal NoAnswerRate => 0;

            public int NoAnswerAverageAttemptScorePercent => 0;

            public double NoAnswerItemTotalCorrelation => 0;

            public double NoAnswerItemRestCoefficient => 0;

            public EmptyAnalysisQuestion(Question q)
            {
                Question = q;
            }

            public IEnumerable<IAttemptAnalysisOption> GetOptions() =>
                Question.Options.Select(x => new EmptyAnalysisOption());
        }

        private class EmptyAnalysisOption : IAttemptAnalysisOption
        {
            public int AttemptCount => 0;

            public int AnswerCount => 0;

            public decimal AnswerRate => 0;

            public int AverageAttemptScorePercent => 0;

            public double ItemTotalCorrelation => 0;

            public double ItemRestCoefficient => 0;
        }

        #endregion

        public void SetInputValues(Question question) =>
            SetInputValues(new EmptyAnalysisQuestion(question));

        public void SetInputValues(IAttemptAnalysisQuestion analysis)
        {
            SuccessRate.Text = string.Format("{0:p0}", analysis.SuccessRate);
            AttemptCount.Text = string.Format("{0:n0}", analysis.AttemptCount);

            AttemptCountLink.NavigateUrl = new ReturnUrl()
                    .GetRedirectUrl($"/ui/admin/assessments/attempts/questions/search?question={analysis.Question.Identifier}");

            NoAnswerRate.Text = string.Format("{0:p0}", analysis.NoAnswerRate);
            NoAnswerCount.Text = string.Format("{0:n0}", analysis.NoAnswerCount);
            NoAnswerAverageAttemptScorePercent.Text = string.Format("{0:n0}", analysis.NoAnswerAverageAttemptScorePercent);
            NoAnswerItemTotalCorrelationText.Text = GetItemTotalCorrelationText(analysis.NoAnswerItemTotalCorrelation);
            NoAnswerItemRestCoefficientText.Text = GetItemRestCoefficientText(analysis.NoAnswerItemRestCoefficient);

            AnswerHeaderRepeater.DataSource = analysis.Question.Options;
            AnswerHeaderRepeater.DataBind();

            var options = analysis.GetOptions();

            AnswerRateRepeater.DataSource = options;
            AnswerRateRepeater.DataBind();

            AnswerCountRepeater.DataSource = options;
            AnswerCountRepeater.DataBind();

            AverageAttemptScoreRepeater.DataSource = options;
            AverageAttemptScoreRepeater.DataBind();

            ItemTotalCorrelationRepeater.DataSource = options;
            ItemTotalCorrelationRepeater.DataBind();

            ItemRestCoefficientRepeater.DataSource = options;
            ItemRestCoefficientRepeater.DataBind();
        }

        protected static string GetItemTotalCorrelationText(double itemTotalCorrelation)
        {
            if (double.IsNaN(itemTotalCorrelation))
            {
                return string.Empty;
            }

            var html = new StringBuilder();

            html.Append($"<span style='color:{GetDiscriminationColor(itemTotalCorrelation)};'>");
            html.Append(GetDiscriminationIcon(itemTotalCorrelation));
            html.Append($" {itemTotalCorrelation:n2}");
            html.Append("</span>");
            return html.ToString();
        }

        protected static string GetItemRestCoefficientText(double itemRestCoefficient)
        {
            return double.IsNaN(itemRestCoefficient)
                ? string.Empty
                : $"{itemRestCoefficient:n2}";
        }

        public static string GetDiscriminationColor(double? correlation)
        {
            if (!correlation.HasValue)
                return "#FFFFFF";

            var value = Math.Round(correlation.Value, 2);

            return value < 0.2 ? "#C0392B" : "#27AE60";
        }

        protected static string GetDiscriminationIcon(double? correlation)
        {
            if (!correlation.HasValue)
                return string.Empty;

            var value = Math.Round(correlation.Value, 2);

            return value < 0.2
                ? "<i class='far fa-exclamation' title='Indicates poor discrimination'></i>"
                : "<i class='far fa-star' title='Indicates good discrimination'></i>";
        }
    }
}