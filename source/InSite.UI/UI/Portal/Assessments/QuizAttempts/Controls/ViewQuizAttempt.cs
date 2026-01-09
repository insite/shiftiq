using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Progresses.Write;
using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.QuizAttempts.Controls
{
    public abstract class ViewQuizAttempt : ViewQuiz
    {
        #region Properties

        protected Guid QuizId
        {
            get => (Guid)ViewState[nameof(QuizId)];
            private set => ViewState[nameof(QuizId)] = value;
        }

        protected Guid AttemptId
        {
            get => (Guid)ViewState[nameof(AttemptId)];
            private set => ViewState[nameof(AttemptId)] = value;
        }

        protected bool CanStart
        {
            get => (bool)(ViewState[nameof(CanStart)] ?? false);
            private set => ViewState[nameof(CanStart)] = value;
        }

        #endregion

        #region Loading

        public override void LoadData(TQuiz quiz, TQuizAttempt attempt)
        {
            CanStart = attempt.QuizIdentifier == quiz.QuizIdentifier
                && attempt.QuizGradebookIdentifier != default
                && attempt.OrganizationIdentifier == Organization.Identifier
                && attempt.LearnerIdentifier == User.Identifier
                && !attempt.AttemptStarted.HasValue
                && attempt.QuizData.IsNotEmpty();

            if (CanStart)
            {
                var attemptLimit = quiz.AttemptLimit;
                if (attemptLimit > 0)
                {
                    var attemptCount = GetAttemptCount(attempt.QuizIdentifier, User.Identifier);
                    CanStart = attemptCount <= attemptLimit;
                }
            }

            if (!CanStart)
                RedirectQuizStart(attempt.QuizIdentifier, false);

            QuizId = attempt.QuizIdentifier;
            AttemptId = attempt.AttemptIdentifier;
        }

        #endregion

        #region Methods (attempt start/complete)

        protected TQuizAttempt GetAttempt()
        {
            var attempt = ServiceLocator.QuizAttemptSearch.Select(AttemptId, x => x.Quiz);

            if (attempt == null)
                RedirectQuizStart(QuizId, false);
            else if (attempt.AttemptCompleted.HasValue)
                RedirectQuizResult(attempt.AttemptIdentifier);

            return attempt;
        }

        protected void StartAttempt(TQuizAttempt attempt)
        {
            if (attempt.AttemptStarted.HasValue)
                throw ApplicationError.Create("The attempt has already been started.");

            attempt.AttemptStarted = DateTimeOffset.Now;

            ServiceLocator.QuizAttemptStore.Update(attempt);
        }

        protected void CompleteQuiz(TQuizAttempt attempt) => CompleteQuiz(attempt, null, 0, DateTimeOffset.Now);

        protected void CompleteQuiz(TQuizAttempt attempt, string text, decimal clientTime, DateTimeOffset completed)
        {
            if (attempt.AttemptCompleted.HasValue)
                throw ApplicationError.Create("The attempt has already been completed.");

            if (!attempt.AttemptStarted.HasValue)
                attempt.AttemptStarted = completed;

            attempt.AttemptCompleted = completed;
            attempt.AttemptData = text;
            attempt.AttemptDuration = clientTime;

            var quizResult = CalculateResult(attempt);
            var activityResult = GetActivityResult(attempt, quizResult);

            attempt.AttemptIsPassing = activityResult.IsPassing;
            attempt.AttemptScore = activityResult.Score;
            attempt.AttemptMistakes = quizResult.Mistakes;
            attempt.AttemptAccuracy = quizResult.Accuracy;
            attempt.AttemptCharsPerMin = quizResult.CharsPerMin;
            attempt.AttemptWordsPerMin = quizResult.WordsPerMin;
            attempt.AttemptKeystrokesPerHour = quizResult.KeystrokesPerHour;
            attempt.AttemptSpeed = quizResult.Speed;

            ServiceLocator.QuizAttemptStore.Update(attempt);

            var commands = new List<Command>();

            UpdateGradebook(attempt.QuizGradebookIdentifier, attempt.LearnerIdentifier, quizResult, commands);

            if (Guid.TryParse(Request.QueryString["activity"], out var activityId))
                UpdateActivity(activityId, attempt, quizResult, commands);

            ServiceLocator.SendCommands(commands);

            RedirectQuizResult(attempt.AttemptIdentifier);
        }

        private void UpdateActivity(Guid activityId, TQuizAttempt attempt, QuizResult quizResult, List<Command> commands)
        {
            var activityGrade = CourseSearch.BindActivityFirst(
                x => new
                {
                    x.GradeItem.Gradebook,
                    x.GradeItem
                },
                x => x.ActivityIdentifier == activityId && x.QuizIdentifier == attempt.QuizIdentifier);
            if (activityGrade == null)
                return;

            var gradebook = activityGrade.Gradebook;
            var gradeItem = activityGrade.GradeItem;
            if (gradebook == null || gradebook.IsLocked)
                return;

            var gradebookId = gradebook.GradebookIdentifier;
            var gradeItemId = gradeItem.GradeItemIdentifier;
            var learnerId = attempt.LearnerIdentifier;

            var gradeSubItems = ServiceLocator.RecordSearch.GetGradeItems(new QGradeItemFilter { ParentGradeItemIdentifier = gradeItemId });
            if (gradeSubItems.IsNotEmpty())
                UpdateProgress(gradeSubItems, learnerId, quizResult, commands);

            var progressId = ServiceLocator.ProgressRestarter.GetProgressIdentifier(learnerId, gradebookId, gradeItemId);
            if (attempt.AttemptIsPassing.Value)
                commands.Add(new CompleteProgress(progressId, DateTimeOffset.UtcNow, attempt.AttemptScore, attempt.AttemptIsPassing.Value, null, null));
            else
                commands.Add(new IncompleteProgress(progressId));

            var calculateCommands = GradebookCalculator.Calculate(gradebookId, learnerId, false, ServiceLocator.RecordSearch);
            commands.AddRange(calculateCommands);
        }

        private (bool IsPassing, decimal Score) GetActivityResult(TQuizAttempt attempt, QuizResult result)
        {
            var pass = true;
            var score = 1m;

            if (attempt.QuizPassingAccuracy > 0)
            {
                pass = pass && result.Accuracy >= attempt.QuizPassingAccuracy;
                score *= Number.CheckRange(result.Accuracy / attempt.QuizPassingAccuracy, 0, 1);
            }

            if (attempt.QuizType == QuizType.TypingSpeed)
            {
                if (attempt.QuizPassingWpm > 0)
                {
                    pass = pass && result.WordsPerMin >= attempt.QuizPassingWpm;
                    score *= Number.CheckRange(result.WordsPerMin / attempt.QuizPassingWpm, 0, 1);
                }
            }
            else if (attempt.QuizType == QuizType.TypingAccuracy)
            {
                if (attempt.QuizPassingKph > 0)
                {
                    pass = pass && result.KeystrokesPerHour >= attempt.QuizPassingKph;
                    score *= Number.CheckRange(result.KeystrokesPerHour / attempt.QuizPassingKph, 0, 1);
                }
            }

            return (pass, score);
        }

        private void UpdateGradebook(Guid gradebookId, Guid learnerId, QuizResult result, List<Command> commands)
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebook(gradebookId, x => x.Items);
            if (gradebook != null && !gradebook.IsLocked)
                UpdateProgress(gradebook.Items, learnerId, result, commands);
        }

        private void UpdateProgress(IEnumerable<QGradeItem> gradeItems, Guid learnerId, QuizResult result, List<Command> commands)
        {
            var itemType = GradeItemType.Score.GetName();

            SetProgress(QuizGradeItem.Accuracy, result.Accuracy);
            SetProgress(QuizGradeItem.Speed, result.Speed);
            SetProgress(QuizGradeItem.Mistakes, result.Mistakes);
            SetProgress(QuizGradeItem.CharsPerMin, result.CharsPerMin);
            SetProgress(QuizGradeItem.WordsPerMin, result.WordsPerMin);
            SetProgress(QuizGradeItem.KeystrokesPerHour, result.KeystrokesPerHour);

            void SetProgress(QuizGradeItem item, decimal value)
            {
                var formatName = item.Format.GetName();
                var gradeItem = gradeItems.FirstOrDefault(x => x.GradeItemName == item.FullName
                    && x.GradeItemType == itemType && x.GradeItemFormat == formatName);
                if (gradeItem == null)
                    return;

                var progressId = ServiceLocator.ProgressRestarter.GetProgressIdentifier(learnerId, gradeItem.GradebookIdentifier, gradeItem.GradeItemIdentifier);

                if (item.Format == GradeItemFormat.Percent)
                    commands.Add(new ChangeProgressPercent(progressId, value, DateTimeOffset.Now));
                else
                    commands.Add(new ChangeProgressNumber(progressId, value, DateTimeOffset.Now));
            }
        }

        protected class QuizResult
        {
            public int Mistakes { get; set; }
            public decimal Accuracy { get; set; }
            public int CharsPerMin { get; set; }
            public int WordsPerMin { get; set; }
            public int KeystrokesPerHour { get; set; }
            public decimal Speed { get; set; }
        }

        protected abstract QuizResult CalculateResult(TQuizAttempt attempt);

        protected virtual void CompareValues(string quizValue, string inputValue, ref int correct, ref int incorrect)
        {
            var charIndex = 0;

            foreach (var ch in quizValue)
            {
                if (charIndex >= inputValue.Length)
                    break;

                if (ch == '\r' || ch == '\f' || ch == '\t' || ch == '\v' || ch == '\0')
                    continue;

                if (inputValue[charIndex] == ch)
                    correct++;
                else
                    incorrect++;

                charIndex += 1;
            }
        }

        #endregion
    }
}