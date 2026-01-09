using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

using InSiteTextBox = InSite.Common.Web.UI.TextBox;

namespace InSite.UI.Portal.Assessments.QuizAttempts.Controls
{
    public partial class ViewTypingAccuracy : ViewQuizAttempt
    {
        private TQuizAttemptTypingAccuracyQuestion[] Questions
        {
            get => (TQuizAttemptTypingAccuracyQuestion[])ViewState[nameof(Questions)];
            set => ViewState[nameof(Questions)] = value;
        }

        private int QuestionIndex
        {
            get => (int)ViewState[nameof(QuestionIndex)];
            set => ViewState[nameof(QuestionIndex)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SampleColumnRepeater.ItemDataBound += SampleColumnRepeater_ItemDataBound;
            InputColumnRepeater.ItemDataBound += InputColumnRepeater_ItemDataBound;

            QuestionPanel.Request += QuestionPanel_Request;
            UpdatePanel.Request += UpdatePanel_Request;
        }

        private void SampleColumnRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var column = (TQuizAttemptTypingAccuracyColumn)e.Item.DataItem;

            var rowRepeater = GetRowRepeater(e.Item);
            rowRepeater.DataSource = column.Rows;
            rowRepeater.DataBind();
        }

        private void InputColumnRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var column = (TQuizAttemptTypingAccuracyColumn)e.Item.DataItem;

            var rowRepeater = GetRowRepeater(e.Item);
            rowRepeater.DataSource = column.Rows;
            rowRepeater.DataBind();
        }

        public override void LoadData(TQuiz quiz, TQuizAttempt attempt)
        {
            base.LoadData(quiz, attempt);

            if (attempt.QuizType != QuizType.TypingAccuracy)
            {
                RedirectQuizStart(attempt.QuizIdentifier, false);
                return;
            }

            Questions = JsonConvert.DeserializeObject<TQuizAttemptTypingAccuracyQuestion[]>(attempt.QuizData);
            QuestionIndex = 0;

            var inputIds = BindInputs();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ViewTypingSpeed),
                "init",
                $"const quizSettings = Object.freeze({{ " +
                $"questionPanelId: '{QuestionPanel.ClientID}', " +
                $"updateId: '{UpdatePanel.ClientID}', " +
                $"loadingId: '{LoadingPanel.ClientID}', " +
                $"timeLimit: {attempt.QuizTimeLimit}" +
                $"}});" +
                $"const quizState = {{ " +
                $"nextId: '{NextButton.ClientID}', " +
                $"completeId: '{CompleteButton.ClientID}', " +
                $"questionIndex: 0, " +
                $"inputIds: ['{string.Join("','", inputIds)}'], " +
                $"}};",
                true);
        }

        private string[] BindInputs()
        {
            var columns = Questions[QuestionIndex].Columns;

            SampleColumnRepeater.DataSource = columns;
            SampleColumnRepeater.DataBind();

            InputColumnRepeater.DataSource = columns;
            InputColumnRepeater.DataBind();

            NextButton.Visible = QuestionIndex < Questions.Length - 1;
            CompleteButton.Visible = QuestionIndex == Questions.Length - 1;

            return EnumerateValueInputs(InputColumnRepeater).Select(x => x.ClientID).ToArray();
        }

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "start")
            {
                var attempt = GetAttempt();
                if (attempt.AttemptStarted.HasValue)
                    CompleteQuiz(attempt);
                else
                    StartAttempt(attempt);
            }
            else if (e.Value.StartsWith("complete"))
            {
                var completedOn = DateTimeOffset.Now;

                var attempt = GetAttempt();
                if (!attempt.AttemptStarted.HasValue)
                    throw ApplicationError.Create("The attempt is not started.");

                var index = e.Value.IndexOf('|', 9);
                var seconds = decimal.Parse(e.Value.Substring(9, index - 9));
                var inputText = e.Value.Substring(index + 1);

                CompleteQuiz(attempt, inputText, seconds, completedOn);
            }
        }

        private void QuestionPanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value != "next")
                return;

            QuestionIndex++;

            var inputIds = BindInputs();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ViewTypingSpeed),
                "reinit",
                $"quizState.nextId = '{NextButton.ClientID}'; " +
                $"quizState.completeId = '{CompleteButton.ClientID}'; " +
                $"quizState.questionIndex = {QuestionIndex}; " +
                $"quizState.inputIds = ['{string.Join("','", inputIds)}'];",
                true);
        }

        protected override QuizResult CalculateResult(TQuizAttempt attempt)
        {
            var clientTime = (double)(attempt.AttemptDuration ?? 0);
            if (clientTime <= 0 || attempt.AttemptData.IsEmpty())
                return new QuizResult();

            var serverTime = (attempt.AttemptCompleted.Value - attempt.AttemptStarted.Value).TotalSeconds;
            if (serverTime <= 0)
                return new QuizResult();

            var quizValues = Questions.SelectMany(x => x.Columns).SelectMany(x => x.Rows).Select(x => x.Value).ToArray();
            var inputValues = JsonConvert.DeserializeObject<List<string>>(attempt.AttemptData);

            if (inputValues.Count > quizValues.Length)
                return new QuizResult();

            while (inputValues.Count < quizValues.Length)
                inputValues.Add(string.Empty);

            var correctCount = 0;
            var incorrectCount = 0;

            for (var i = 0; i < quizValues.Length; i++)
                CompareValues(quizValues[i], inputValues[i], ref correctCount, ref incorrectCount);

            var quizInteval = serverTime - clientTime >= 2 ? serverTime : clientTime;
            var charPerMin = 60d / quizInteval * correctCount;
            var wordPerMin = charPerMin / 5;
            var keystrokePerHour = wordPerMin * 5 * 60;

            var accuracy = 0m;
            if (correctCount + incorrectCount > 0)
                accuracy = (decimal)correctCount / (correctCount + incorrectCount);

            var speed = 0m;
            var requiredSpeed = attempt.Quiz?.PassingPoints ?? 0;
            if (requiredSpeed > 0)
                speed = (decimal)wordPerMin / requiredSpeed;

            return new QuizResult
            {
                Mistakes = incorrectCount,
                CharsPerMin = (int)Math.Round(charPerMin, MidpointRounding.AwayFromZero),
                WordsPerMin = (int)Math.Round(wordPerMin, MidpointRounding.AwayFromZero),
                KeystrokesPerHour = (int)Math.Round(keystrokePerHour, MidpointRounding.AwayFromZero),
                Accuracy = accuracy,
                Speed = speed
            };
        }

        protected override void CompareValues(string quizValue, string inputValue, ref int correct, ref int incorrect)
        {
            base.CompareValues(quizValue, inputValue, ref correct, ref incorrect);

            incorrect += Math.Abs(inputValue.Length - quizValue.Length);
        }

        private static Repeater GetRowRepeater(RepeaterItem item)
        {
            return (Repeater)item.FindControl("RowRepeater");
        }

        private static IEnumerable<InSiteTextBox> EnumerateValueInputs(Repeater columnRepeater)
        {
            return columnRepeater.Items.Cast<RepeaterItem>()
                .Where(x => IsContentItem(x))
                .SelectMany(x => GetRowRepeater(x).Items.Cast<RepeaterItem>())
                .Where(x => IsContentItem(x))
                .Select(x => (InSiteTextBox)x.FindControl("Value"));
        }
    }
}