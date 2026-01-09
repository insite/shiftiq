using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;

using Shift.Constant;

namespace InSite.Domain.Attempts
{
    public class AttemptAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new AttemptState();

        public AttemptState Data => (AttemptState)State;

        public const string ErrorPingOutdated = "The command cannot be executed because the ping timestamp is out of date";

        #region Methods (commands)

        public void AnalyzeAttempt(bool allowTakeAttendance)
        {
            var e = new AttemptAnalyzed(allowTakeAttendance);

            Apply(e);
        }

        public void AnswerComposedQuestion(Guid question, string answer)
        {
            if (!CanAnswerQuestion(question))
                return;

            var e = new ComposedQuestionAnswered(question, answer);

            Apply(e);
        }

        public void AnswerMatchingQuestion(Guid question, IDictionary<int, string> matches)
        {
            if (!CanAnswerQuestion(question))
                return;

            var e = new MatchingQuestionAnswered(question, matches);

            Apply(e);
        }

        public void AnswerMultipleChoiceQuestion(Guid question, int option)
        {
            if (!CanAnswerQuestion(question))
                return;

            var e = new MultipleChoiceQuestionAnswered(question, option);

            Apply(e);
        }

        public void AnswerMultipleCorrectQuestion(Guid question, int[] options)
        {
            if (!CanAnswerQuestion(question))
                return;

            var e = new MultipleCorrectQuestionAnswered(question, options);

            Apply(e);
        }

        public void AnswerBooleanTableQuestion(Guid question, IDictionary<int, bool> options)
        {
            if (!CanAnswerQuestion(question))
                return;

            var e = new BooleanTableQuestionAnswered(question, options);

            Apply(e);
        }

        public void AnswerTrueOrFalseQuestion(Guid question, int option)
        {
            if (!CanAnswerQuestion(question))
                return;

            Apply(new TrueOrFalseQuestionAnswered(question, option));
        }

        public void AnswerHotspotQuestion(Guid question, AttemptHotspotPinAnswer[] pins)
        {
            if (pins.Length == 0 || pins.Any(p => p.X < 0 || p.X > 1 || p.Y < 0 || p.Y > 1))
                return;

            if (!CanAnswerQuestion(question))
                return;

            var e = new HotspotQuestionAnswered(question, pins);

            Apply(e);
        }

        public void AnswerOrderingQuestion(Guid question, int[] optionsOrder)
        {
            if (optionsOrder.IsEmpty() || !CanAnswerQuestion(question))
                return;

            var e = new OrderingQuestionAnswered(question, optionsOrder);

            Apply(e);
        }

        public void AuthorComment(Guid question, string comment)
        {
            if (!CanAnswerQuestion(question))
                return;

            var e = new AttemptCommentPosted(question, comment);

            Apply(e);
        }

        public void CalculateScore(decimal points, decimal score, string grade, bool isPassing)
        {
            var e = new ScoreCalculated(points, score, grade, isPassing);

            Apply(e);
        }

        public void SubmitAttempt(string userAgent, bool grade)
        {
            if (Data.Submitted.HasValue || !Data.Started.HasValue)
                return;

            var e = new AttemptSubmitted(userAgent, grade);

            Apply(e);
        }

        public void GradeAttempt()
        {
            var e = new AttemptGraded(null);

            Apply(e);
        }

        public void FixAttempt(int? points, int? score, bool? isPassing, Guid? registration)
        {
            var e = new AttemptFixed(points, score, isPassing, registration);

            Apply(e);
        }

        public void ImportAttempt(
            Guid organization,
            AnswerHandle[] answers,
            DateTimeOffset? started,
            DateTimeOffset? completed,
            string tag,
            Guid form,
            Guid candidate,
            Guid? registration,
            bool isAttended,
            string language
            )
        {
            if (Data != null)
                return;

            var e = new AttemptImported(organization, answers, started, completed, tag, form, candidate, registration, isAttended, language);

            Apply(e);
        }

        public void PingAttempt()
        {
            CheckPingTimestamp();

            var e = new AttemptPinged();

            Apply(e);
        }

        public void ResumeAttempt(int? pingInterval)
        {
            if (Data.Submitted.HasValue || !Data.Pinged.HasValue && !Data.Started.HasValue || !IsPingExpired())
                return;

            var e = new AttemptResumed(pingInterval);

            Apply(e);
        }

        public void ScoreComposedQuestion(Guid questionId, Dictionary<Guid, decimal> rubricRatingPoints)
        {
            if (rubricRatingPoints.IsEmpty())
                return;

            if (questionId == default)
                throw ApplicationError.Create("QuestionIdentifier is empty");

            if (!Data.Questions.Any(x => x.QuestionIdentifier == questionId))
                throw ApplicationError.Create("QuestionIdentifier not found");

            if (rubricRatingPoints.Any(x => x.Key == default))
                throw ApplicationError.Create("RatingIdentifier is empty");

            var e = new ComposedQuestionScored(questionId, rubricRatingPoints);

            Apply(e);
        }

        public void StartAttempt(Guid organizationId, Guid bankId, Guid formId, Guid assessorId, Guid learnerId, Guid? registrationId, string userAgent, AttemptConfiguration config, AttemptSection[] sections, AttemptQuestion[] questions)
        {
            if (Data != null)
                return;

            if (organizationId == default)
                throw ApplicationError.Create("OrganizationIdentifier is empty");

            if (bankId == default)
                throw ApplicationError.Create("BankIdentifier is empty");

            if (formId == default)
                throw ApplicationError.Create("FormIdentifier is empty");

            if (assessorId == default)
                throw ApplicationError.Create("AssessorIdentifier is empty");

            if (learnerId == default)
                throw ApplicationError.Create("LearnerIdentifier is empty");

            if (registrationId.HasValue && registrationId.Value == default)
                throw ApplicationError.Create("RegistrationIdentifier is empty");

            if (questions.Length == 0)
                throw ApplicationError.Create("Attempt questions are not defined");

            if (questions.Any(x => x.Identifier == default))
                throw ApplicationError.Create("QuestionIdentifier is empty");

            if (questions.GroupBy(x => x.Identifier).Any(x => x.Count() > 1))
                throw ApplicationError.Create("QuestionIdentifier must be unique");

            if (!config.SectionsAsTabs)
                config.TabNavigation = true;

            if (config.TabNavigation)
                config.SingleQuestionPerTab = false;

            if (config.SectionsAsTabs && !config.TabNavigation)
            {
                var sectionLen = sections.Length;
                if (sectionLen == 0)
                    throw ApplicationError.Create("Attempt sections are not defined");

                if (questions.Any(x => !x.Section.HasValue))
                    throw ApplicationError.Create("AttemptQuestion.Section is required value");

                if (questions.Any(x => x.Section.Value < 0 || x.Section.Value >= sectionLen))
                    throw ApplicationError.Create("AttemptQuestion.Section refers to an undefined section");

                if (config.TabTimeLimit == SpecificationTabTimeLimit.AllTabs)
                {
                    config.TimeLimit = null;

                    if (sections.Any(x => x.TimeLimit <= 0))
                        throw ApplicationError.Create("AttemptSection.TimeLimit is required value");
                }
            }
            else
            {
                sections = new AttemptSection[0];
                foreach (var q in questions)
                    q.Section = null;
            }

            var e = new AttemptStarted3(organizationId, bankId, formId, assessorId, learnerId, registrationId, userAgent, config, sections, questions);

            Apply(e);
        }

        public void StartComposedQuestionAttempt(Guid question)
        {
            if (!CanAnswerQuestion(question))
                return;

            var e = new ComposedQuestionAttemptStarted(question);

            Apply(e);
        }

        public void TagAttempt(string tag)
        {
            var e = new AttemptTagged(tag);

            Apply(e);
        }

        public void VoidAttempt(string reason)
        {
            Apply(new AttemptVoided(reason));
        }

        public void VoidQuestion(Guid question)
        {
            var e = new QuestionVoided(question);

            Apply(e);
        }

        public void RegradeQuestion(Guid form, Guid question, List<OldOption> oldOptions, RegradeOption regradeOption)
        {
            var e = new QuestionRegraded(form, question, oldOptions, regradeOption);

            Apply(e);
        }

        public void ChangeAttemptCompleteDate(DateTimeOffset completed)
        {
            var e = new AttemptGradedDateChanged(completed);

            Apply(e);
        }

        public void ChangeAttemptGradingAssessor(Guid? gradingAssessor)
        {
            var e = new AttemptGradingAssessorAssigned(gradingAssessor);

            Apply(e);
        }

        public void SwitchAttemptSection(int nextSectionIndex)
        {
            if (Data.Submitted.HasValue || !Data.Started.HasValue)
                return;

            CheckPingTimestamp();

            var config = Data.Configuration;
            if (!config.SectionsAsTabs || config.TabNavigation)
                return;

            if (Data.Sections.IsEmpty() || !Data.ActiveSectionIndex.HasValue)
                return;

            if (nextSectionIndex >= Data.Sections.Length || nextSectionIndex != Data.ActiveSectionIndex.Value + 1)
                return;

            var e = new AttemptSectionSwitched(nextSectionIndex);

            Apply(e);
        }

        public void SwitchAttemptQuestion(int questionIndex)
        {
            if (Data.Submitted.HasValue || !Data.Started.HasValue)
                return;

            CheckPingTimestamp();

            var config = Data.Configuration;
            if (!config.SectionsAsTabs || config.TabNavigation || !config.SingleQuestionPerTab)
                return;

            if (Data.Sections.IsEmpty() || !Data.ActiveSectionIndex.HasValue || !Data.ActiveQuestionIndex.HasValue)
                return;

            if (questionIndex != Data.ActiveQuestionIndex.Value + 1)
                return;

            var question = Data.Questions.FirstOrDefault(x => x.QuestionIndex >= questionIndex);
            if (question == null)
                return;

            var section = question.SectionIndex;
            if (!section.HasValue || section.Value < Data.ActiveSectionIndex.Value)
                return;

            if (section.Value > Data.ActiveSectionIndex.Value + 1)
            {
                section = Data.ActiveSectionIndex.Value + 1;
                questionIndex = Data.ActiveQuestionIndex.Value;
            }
            else
            {
                questionIndex = question.QuestionIndex;
            }

            var e = new AttemptQuestionSwitched(section.Value, questionIndex);

            Apply(e);
        }

        public void UpdateAttemptRubricPoints(Guid rubricId, decimal rubricPoints)
        {
            if (rubricId == default || rubricPoints <= 0)
                return;

            var questions = Data.Questions
                .Where(x => x.Rubric != null && x.Rubric.Identifier == rubricId && x.Rubric.Points != rubricPoints)
                .Select(x => x.QuestionIdentifier)
                .ToArray();
            if (questions.IsEmpty())
                return;

            var e = new AttemptRubricPointsUpdated(questions, rubricId, rubricPoints);

            Apply(e);
        }

        public void ChangeAttemptQuestionRubric(Guid questionId, AttemptQuestionRubric rubric)
        {
            if (questionId == default || rubric == null || rubric.Identifier == default || rubric.Points <= 0)
                return;

            var question = Data.Questions.FirstOrDefault(x => x.QuestionIdentifier == questionId);
            if (question?.Rubric == null || question.Rubric.Identifier == rubric.Identifier)
                return;

            var e = new AttemptQuestionRubricChanged(questionId, rubric);

            Apply(e);
        }

        public void InitAttemptQuestionRubric(Guid questionId, AttemptQuestionRubric rubric)
        {
            if (questionId == default || rubric == null || rubric.Identifier == default || rubric.Points <= 0)
                return;

            var question = Data.Questions.FirstOrDefault(x => x.QuestionIdentifier == questionId);
            if (question == null || question.Rubric != null)
                return;

            var e = new AttemptQuestionRubricInited(questionId, rubric);

            Apply(e);
        }

        #endregion

        #region Methods (helpers)

        private bool CanAnswerQuestion(Guid question)
        {
            if (Data.Submitted.HasValue || !Data.Started.HasValue)
                return false;

            CheckPingTimestamp();

            var now = DateTimeOffset.Now;
            var config = Data.Configuration;

            if (config.TimeLimit > 0)
            {
                var attemptDuration = Data.TimeIntervals.Sum(x => x.GetDuration(now));
                if (attemptDuration > (config.TimeLimit * 60 + 5))
                    return false;
            }

            if (!config.SectionsAsTabs || config.TabNavigation)
                return true;

            var qState = Data.Questions.FirstOrDefault(x => x.QuestionIdentifier == question
                                                         || x.SubQuestions.Contains(question));
            if (qState == null)
                return false;

            var isSectionMatch = qState.SectionIndex.HasValue && Data.ActiveSectionIndex.HasValue
                && qState.SectionIndex.Value == Data.ActiveSectionIndex.Value;
            if (!isSectionMatch)
                return false;

            if (config.TabTimeLimit == SpecificationTabTimeLimit.AllTabs)
            {
                var section = Data.Sections[Data.ActiveSectionIndex.Value];
                var sectionDuration = Data.TimeIntervals
                    .Where(x => x.SectionIndex == section.Index)
                    .Sum(x => x.GetDuration(now));

                if (sectionDuration > (section.TimeLimit * 60 + 5))
                    return false;
            }

            if (!config.SingleQuestionPerTab)
                return true;

            return Data.ActiveQuestionIndex.HasValue
                && qState.QuestionIndex == Data.ActiveQuestionIndex.Value;
        }

        private void CheckPingTimestamp()
        {
            if (IsPingExpired())
                throw ApplicationError.Create(ErrorPingOutdated);
        }

        private bool IsPingExpired()
        {
            var pingDate = Data.Pinged ?? Data.Started.Value;
            return (DateTimeOffset.UtcNow - pingDate).TotalSeconds > Data.Configuration.PingInterval * 2;
        }

        #endregion
    }
}
