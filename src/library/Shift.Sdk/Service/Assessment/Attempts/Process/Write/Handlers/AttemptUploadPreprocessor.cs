using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.File;

using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Application.Attempts.Write
{
    public class AttemptUploadPreprocessor
    {
        private IBankSearch _assessments;
        private IAttemptSearch _attempts;
        private Func<Guid, string, Guid> _getUserIdentifier;

        public string Error { get; set; }
        public List<string> Warnings { get; set; }
        public ListItemArray Summaries { get; set; }

        public AttemptUploadPreprocessor(IBankSearch assessments, IAttemptSearch attempts, Func<Guid, string, Guid> getUserIdentifier)
        {
            _assessments = assessments;
            _attempts = attempts;
            _getUserIdentifier = getUserIdentifier;
        }

        public AttemptUploadAnswer[] Process(AttemptUploadFileLine[] lines, TimeZoneInfo defaultTimeZone, Guid? defaultEvent, Guid? defaultForm)
        {
            Warnings = new List<string>();
            Summaries = new ListItemArray();

            var attempts = new List<AttemptUploadAnswer>();
            var attemptMap = new Dictionary<string, AttemptUploadAnswer>(StringComparer.OrdinalIgnoreCase);
            var registrations = _assessments.GetAssessmentFormRegistrations(defaultEvent);
            var internalWarnings = new List<string>();

            for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                var line = lines[lineNumber];
                var hasLearnerCode = !string.IsNullOrWhiteSpace(line.LearnerCode);
                var userRegistration = hasLearnerCode && registrations != null
                    ? registrations.FirstOrDefault(x => string.Equals(x.LearnerPersonCode, line.LearnerCode, StringComparison.OrdinalIgnoreCase))
                    : null;
                var formId = userRegistration != null ? userRegistration.AssessmentFormIdentifier : defaultForm;
                var form = _assessments.GetFormData(formId.Value);
                var questions = form.GetQuestions();

                if (line.AttemptAnswers.Length < questions.Count)
                {
                    Error = $"Line {1 + lineNumber} contains {ShiftHumanizer.ToQuantity(line.AttemptAnswers.Length, "answer")} instead of the expected {ShiftHumanizer.ToQuantity(questions.Count, "answer")}.";
                    return attempts.ToArray();
                }

                if (line.AttemptAnswers.Length > questions.Count)
                    internalWarnings.Add(
                        $"Line {1 + lineNumber} contains <b>{ShiftHumanizer.ToQuantity(line.AttemptAnswers.Length, "answer")}</b> " +
                        $"instead of the expected <b>{ShiftHumanizer.ToQuantity(questions.Count, "answer")}</b>.");

                if (!hasLearnerCode || !attemptMap.TryGetValue(line.LearnerCode, out var attempt))
                {
                    var userId = _getUserIdentifier(form.Specification.Bank.Tenant, line.LearnerCode);

                    attempt = new AttemptUploadAnswer
                    {
                        FormIdentifier = formId.Value,
                        FormName = form.Name,
                        Sequence = attempts.Count + 1,
                        LearnerCode = line.LearnerCode,
                        LearnerName = line.LearnerName,
                        LearnerUserIdentifier = userId,
                        AttemptGraded = line.AttemptDate.HasValue
                            ? new DateTimeOffset(line.AttemptDate.Value, defaultTimeZone.GetUtcOffset(line.AttemptDate.Value))
                            : (DateTimeOffset?)null,
                        Tag = line.Instructor,
                        HasEventRegistration = userRegistration != null,
                        HasUserAccount = userId != Guid.Empty,
                        Questions = new AttemptUploadAnswer.Question[questions.Count]
                    };

                    attempts.Add(attempt);

                    if (hasLearnerCode)
                        attemptMap.Add(line.LearnerCode, attempt);
                }

                GetQuestions(line, attempt.Questions);
                FinalizeAttempt(attempt);
                CalculateScores(form, attempt);
                LoadCurrentAttempt(form, attempt);
            }

            var array = attempts.ToArray();

            CalculateSummaries(array);

            Warnings.AddRange(internalWarnings);

            return array;
        }

        private void GetQuestions(AttemptUploadFileLine line, AttemptUploadAnswer.Question[] attemptQuestions)
        {
            for (var questionIndex = 0; questionIndex < attemptQuestions.Length; questionIndex++)
            {
                var attemptQuestion = attemptQuestions[questionIndex];

                if (attemptQuestion == null)
                {
                    attemptQuestion = new AttemptUploadAnswer.Question
                    {
                        Sequence = questionIndex + 1,
                        Answers = new List<string>()
                    };

                    attemptQuestions[questionIndex] = attemptQuestion;
                }

                attemptQuestion.Answers.Add(line.AttemptAnswers[questionIndex]);
            }
        }

        private static void FinalizeAttempt(AttemptUploadAnswer attempt)
        {
            var errors = new HashSet<string>();

            if (!attempt.HasUserAccount)
                errors.Add("DAID");

            foreach (var q in attempt.Questions)
            {
                var invalidAnswers = q.Answers.Where(x => !string.IsNullOrWhiteSpace(x) && x != "-" && !AttemptUploadAnswer.IsValidAnswer(x)).ToList();
                foreach (var invalidAnswer in invalidAnswers)
                    errors.Add($"{invalidAnswer}");

                var hasEmpty = q.Answers.Any(x => string.IsNullOrWhiteSpace(x));
                if (hasEmpty)
                    errors.Add("()");

                if (invalidAnswers.Count == 0 && !hasEmpty)
                {
                    var firstAnswer = q.Answers[0];

                    if (q.Answers.All(x => string.Equals(x, firstAnswer, StringComparison.OrdinalIgnoreCase)))
                        q.FinalAnswer = firstAnswer;
                    else
                        errors.Add("!");
                }
            }

            if (errors.Count > 0)
                attempt.Errors = string.Join(" ", errors.OrderBy(x => x));

            attempt.IsAttended = attempt.Questions.All(x => x.IsValid);
        }

        private static void CalculateScores(Form form, AttemptUploadAnswer attempt)
        {
            var formQuestions = form.GetQuestions();

            var points = 0m;
            var sumPoints = 0m;

            for (int i = 0; i < attempt.Questions.Length && i < formQuestions.Count; i++)
            {
                var formQuestion = formQuestions[i];
                var attemptQuestion = attempt.Questions[i];

                attemptQuestion.CorrectAnswer = string.Empty;

                if (formQuestion.Options.Count == 0)
                    continue;

                var maxPoints = formQuestion.Options.Max(x => x.Points);

                sumPoints += maxPoints;

                var correctAnswerIndex = formQuestion.Options.FindIndex(x => x.HasPoints && x.Points == maxPoints);
                if (correctAnswerIndex >= 0)
                    attemptQuestion.CorrectAnswer = Calculator.ToBase26(correctAnswerIndex + 1);

                var optionIndex = attemptQuestion.OptionIndex;
                if (optionIndex >= 0 && optionIndex < formQuestion.Options.Count)
                {
                    var option = formQuestion.Options[optionIndex];
                    if (option != null)
                        points += option.Points;
                }
            }

            if (!attempt.Questions.Any(x => x.FinalAnswer.IsEmpty()))
            {
                attempt.Percent = sumPoints == 0 ? 0 : Calculator.GetPercentDecimal(points, sumPoints);
                attempt.IsPassed = form.Specification.Calculation.PassingScore <= attempt.Percent;
            }
        }

        public void CalculateScores(AttemptUploadAnswer[] attempts)
        {
            foreach (var attempt in attempts)
            {
                var form = _assessments.GetFormData(attempt.FormIdentifier.Value);
                CalculateScores(form, attempt);
            }
        }

        private void LoadCurrentAttempt(Form form, AttemptUploadAnswer attempt)
        {
            var attempts = _attempts.GetAttempts(new QAttemptFilter
            {
                LearnerUserIdentifier = attempt.LearnerUserIdentifier,
                FormIdentifier = form.Identifier
            });

            if (attempts.Count == 0)
                return;

            attempts.Sort((a, b) => -DateTimeOffset.Compare(a.AttemptGraded ?? DateTimeOffset.MinValue, b.AttemptGraded ?? DateTimeOffset.MinValue));

            var mostRecentAttemptId = attempts.First().AttemptIdentifier;
            var mostRecentAttempt = _attempts.GetAttempt(mostRecentAttemptId, x => x.Questions);

            var formQuestions = form.GetQuestions();

            for (int i = 0; i < formQuestions.Count; i++)
            {
                var formQuestion = formQuestions[i];
                var attemptQuestion = attempt.Questions[i];

                var latestQuestion = mostRecentAttempt.Questions.FirstOrDefault(x => x.QuestionIdentifier == formQuestion.Identifier);
                if (latestQuestion != null && latestQuestion.AnswerOptionKey.HasValue)
                {
                    var formOptionIndex = formQuestion.Options.FindIndex(x => x.Number == latestQuestion.AnswerOptionKey);
                    attemptQuestion.CurrentAnswer = formOptionIndex >= 0 ? ((char)('A' + formOptionIndex)).ToString() : "";

                    attempt.HasCurrentAttempt = true;
                }
            }
        }

        private void CalculateSummaries(AttemptUploadAnswer[] attempts)
        {
            if (attempts.Length == 0)
                return;

            var halfCount = attempts.Length / 2;
            var calculatorData = attempts.Select(x => (double)x.Percent);

            Summaries.Add(new ListItem { Text = "Exam Attempts", Value = attempts.Length.ToString() });
            Summaries.Add(new ListItem { Text = "Exam Candidates", Value = attempts.Select(x => x.LearnerCode).Distinct(StringComparer.OrdinalIgnoreCase).Count().ToString() });
            Summaries.Add(new ListItem { Text = "High Score", Value = attempts.Max(x => x.Percent).ToString("p0") });
            Summaries.Add(new ListItem { Text = "Low Score", Value = attempts.Min(x => x.Percent).ToString("p0") });
            Summaries.Add(new ListItem { Text = "Mean Score", Value = attempts.Average(x => x.Percent).ToString("p0") });
            Summaries.Add(new ListItem { Text = "Median Score", Value = Calculator.CalculateMedian(calculatorData).ToString("p0") });
            Summaries.Add(new ListItem { Text = "Standard Deviation", Value = Calculator.CalculateStandardDeviation(calculatorData).ToString("p0") });

            var below30 = attempts.Count(x => x.Percent < .3m);
            if (below30 > 0)
                Warnings.Add($"{ShiftHumanizer.ToQuantity(below30, "exam attempt")} with a score less than 30%");

            var failure = attempts.Count(x => !x.IsPassed);
            if (failure >= halfCount)
                Warnings.Add($"{ShiftHumanizer.ToQuantity(failure, "exam attempts")} (out of {attempts.Length}) with a failing score");

            var lowScoreFailure = attempts.Count(x => !x.IsPassed && x.Percent < .6m);
            if (lowScoreFailure >= halfCount)
            {
                var text = $"{ShiftHumanizer.ToQuantity(lowScoreFailure, "exam attempts")} in this group ({Calculator.GetPercentDecimal(lowScoreFailure, attempts.Length):p0}) with a failing score less than 60%";
                Warnings.Add(text);
            }

            var highScoreFailure = attempts.Count(x => !x.IsPassed && x.Percent >= .6m);
            if (highScoreFailure >= halfCount)
                Warnings.Add($"{ShiftHumanizer.ToQuantity(highScoreFailure, "exam attempts")} in this group ({Calculator.GetPercentDecimal(highScoreFailure, attempts.Length):p0}) with a failing score of 60% or higher");
        }
    }
}