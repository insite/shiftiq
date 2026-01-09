using System;
using System.Linq;

using Humanizer;
using Humanizer.Localisation;

using InSite.Application.QuizAttempts.Read;
using InSite.Application.Quizzes.Read;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.QuizAttempts.Controls
{
    public partial class ViewWelcome : ViewQuiz
    {
        [Serializable]
        private class QuizInfo
        {
            public Guid QuizIdentifier { get; }
            public Guid GradebookIdentifier { get; }

            public string QuizType { get; }
            public string QuizName { get; }
            public string QuizData { get; }
            public int TimeLimit { get; }
            public int AttemptLimit { get; }
            public decimal PassingAccuracy { get; }
            public int PassingWpm { get; }
            public int PassingKph { get; }

            public QuizInfo(TQuiz quiz)
            {
                QuizIdentifier = quiz.QuizIdentifier;
                GradebookIdentifier = quiz.GradebookIdentifier;
                QuizType = quiz.QuizType;
                QuizName = quiz.QuizName;
                QuizData = quiz.QuizData;
                TimeLimit = quiz.TimeLimit;
                AttemptLimit = quiz.AttemptLimit;
                PassingAccuracy = quiz.PassingAccuracy;
                PassingWpm = quiz.PassingWpm;
                PassingKph = quiz.PassingKph;
            }
        }

        private QuizInfo Quiz
        {
            get => (QuizInfo)ViewState[nameof(Quiz)];
            set => ViewState[nameof(Quiz)] = value;
        }

        private bool CanStart
        {
            get => (bool)(ViewState[nameof(CanStart)] ?? false);
            set => ViewState[nameof(CanStart)] = value;
        }

        public bool NeedRestart { get; internal set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StartButton.Click += StartButton_Click;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (CanStart)
                SetupAttemptLimit();

            if (!CanStart)
                return;

            var attempt = CreateAttempt(Quiz);

            ServiceLocator.QuizAttemptStore.Insert(attempt);

            RedirectQuizAttempt(attempt.AttemptIdentifier);
        }

        public override void LoadData(TQuiz quiz, TQuizAttempt attempt)
        {
            if (!NeedRestart)
                TryRedirectToAttempt(quiz.QuizIdentifier, User.UserIdentifier);

            Quiz = new QuizInfo(quiz);
            CanStart = true;

            LoadQuiz(quiz);
            LoadLearner(User.UserIdentifier);
            SetupAttemptLimit();

            StartButton.Enabled = CanStart;
        }

        private void LoadQuiz(TQuiz quiz)
        {
            QuizType.Text = quiz.QuizType;
            QuizTimeLimitField.Visible = quiz.TimeLimit > 0;
            QuizTimeLimit.Text = quiz.TimeLimit.Seconds()
                .Humanize(precision: 2, maxUnit: TimeUnit.Minute, minUnit: TimeUnit.Second);
        }

        private void LoadLearner(Guid learnerId)
        {
            var learner = ServiceLocator.ContactSearch.GetPerson(learnerId, Organization.Identifier);

            LearnerFirstName.Text = learner.UserFirstName;
            LearnerLastName.Text = learner.UserLastName;
            LearnerEmail.Text = learner.UserEmail;
        }

        private void SetupAttemptLimit()
        {
            QuizAttemptLimitField.Visible = false;
            QuizAttemptLeftField.Visible = false;

            var attemptLimit = Quiz.AttemptLimit;
            if (attemptLimit <= 0)
                return;

            var attemptCount = GetAttemptCount(Quiz.QuizIdentifier, User.Identifier);

            if (attemptCount == 0)
            {
                QuizAttemptLimitField.Visible = true;
                QuizAttemptLimit.Text = "attempt".ToQuantity(attemptLimit);
            }
            else if (attemptCount < attemptLimit)
            {
                var leftCount = attemptLimit - attemptCount;

                QuizAttemptLeftField.Visible = true;
                QuizAttemptLeft.Text = leftCount == 1
                    ? "Last attempt"
                    : "attempt".ToQuantity(leftCount);
            }
            else
            {
                CanStart = false;
                OnAlert(AlertType.Error, "You have reached the maximum number of attempts for this quiz.");
            }
        }

        private void TryRedirectToAttempt(Guid quizId, Guid learnerId)
        {
            var latestAttempt = ServiceLocator.QuizAttemptSearch.SelectLatest(new TQuizAttemptFilter
            {
                QuizIdentifier = quizId,
                LearnerIdentifier = learnerId
            });

            if (latestAttempt == null)
                return;

            if (!latestAttempt.AttemptStarted.HasValue)
                RedirectQuizAttempt(latestAttempt.AttemptIdentifier);
            else
                RedirectQuizResult(latestAttempt.AttemptIdentifier);
        }

        private static TQuizAttempt CreateAttempt(QuizInfo quiz)
        {
            var quizType = quiz.QuizType;

            return new TQuizAttempt
            {
                AttemptIdentifier = UniqueIdentifier.Create(),
                OrganizationIdentifier = Organization.Identifier,
                QuizIdentifier = quiz.QuizIdentifier,
                LearnerIdentifier = User.Identifier,
                AttemptCreated = DateTimeOffset.Now,
                QuizGradebookIdentifier = quiz.GradebookIdentifier,
                QuizType = quizType,
                QuizName = quiz.QuizName,
                QuizData = quizType == Shift.Constant.QuizType.TypingSpeed
                    ? GetTypingSpeedQuizData(quiz)
                    : quizType == Shift.Constant.QuizType.TypingAccuracy
                        ? GetTypingAccuracyQuizData(quiz)
                        : throw ApplicationError.Create("Unexpected quiz type: " + quizType),
                QuizTimeLimit = quiz.TimeLimit,
                QuizPassingAccuracy = quiz.PassingAccuracy,
                QuizPassingWpm = quiz.PassingWpm,
                QuizPassingKph = quiz.PassingKph
            };
        }

        private static string GetTypingSpeedQuizData(QuizInfo quiz)
        {
            var data = JsonConvert.DeserializeObject<string[]>(quiz.QuizData);
            var index = RandomNumberGenerator.Instance.Next(0, data.Length);

            return data[index];
        }

        private static string GetTypingAccuracyQuizData(QuizInfo quiz)
        {
            var data = JsonConvert.DeserializeObject<TQuizTypingAccuracyQuestion[]>(quiz.QuizData);
            var questions = data.Select(q => new TQuizAttemptTypingAccuracyQuestion
            {
                Columns = q.Columns.Select(c => new TQuizAttemptTypingAccuracyColumn
                {
                    Rows = c.Rows.Select(r =>
                    {
                        var index = RandomNumberGenerator.Instance.Next(0, r.Values.Count);

                        return new TQuizAttemptTypingAccuracyRow
                        {
                            Label = r.Label,
                            Value = r.Values[index]
                        };
                    }).ToList()
                }).ToList()
            }).ToArray();

            return JsonConvert.SerializeObject(questions);
        }
    }
}