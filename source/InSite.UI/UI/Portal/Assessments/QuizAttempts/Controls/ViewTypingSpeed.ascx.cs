using System;
using System.Web.UI;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;

using Shift.Common;
using Shift.Common.Events;

using Shift.Constant;

namespace InSite.UI.Portal.Assessments.QuizAttempts.Controls
{
    public partial class ViewTypingSpeed : ViewQuizAttempt
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UpdatePanel.Request += UpdatePanel_Request;
        }

        public override void LoadData(TQuiz quiz, TQuizAttempt attempt)
        {
            base.LoadData(quiz, attempt);

            if (attempt.QuizType != QuizType.TypingSpeed)
            {
                RedirectQuizStart(attempt.QuizIdentifier, false);
                return;
            }

            QuizText.InnerText = attempt.QuizData;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ViewTypingSpeed),
                "init",
                $"const quizSettings = Object.freeze({{ " +
                $"textId: '{QuizText.ClientID}', " +
                $"updateId: '{UpdatePanel.ClientID}', " +
                $"loadingId: '{LoadingPanel.ClientID}', " +
                $"timeLimit: {attempt.QuizTimeLimit}" +
                $"}});",
                true);
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
                var text = e.Value.Substring(index + 1);

                CompleteQuiz(attempt, text, seconds, completedOn);
            }
        }

        protected override QuizResult CalculateResult(TQuizAttempt attempt)
        {
            var clientTime = (double)(attempt.AttemptDuration ?? 0);
            if (clientTime <= 0)
                return new QuizResult();

            var serverTime = (attempt.AttemptCompleted.Value - attempt.AttemptStarted.Value).TotalSeconds;
            if (serverTime <= 0)
                return new QuizResult();

            var correctCount = 0;
            var incorrectCount = 0;

            CompareValues(QuizText.InnerText, attempt.AttemptData, ref correctCount, ref incorrectCount);

            var quizInteval = serverTime - clientTime >= 2 ? serverTime : clientTime;
            var charPerMin = 60d / quizInteval * correctCount;
            var wordPerMin = charPerMin / 5;

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
                Accuracy = accuracy,
                Speed = speed
            };
        }
    }
}