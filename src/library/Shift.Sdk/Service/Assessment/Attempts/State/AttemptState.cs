using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Attempts
{
    public class AttemptState : AggregateState
    {
        [JsonProperty]
        public Guid Identifier { get; private set; }

        [JsonProperty]
        public Guid? BankIdentifier { get; private set; }

        [JsonProperty]
        public AttemptConfiguration Configuration { get; private set; }

        [JsonProperty]
        public AttemptSectionState[] Sections { get; private set; }

        [JsonProperty]
        public AttemptQuestionState[] Questions { get; private set; }

        [JsonProperty]
        public int? ActiveSectionIndex { get; private set; }

        [JsonProperty]
        public int? ActiveQuestionIndex { get; private set; }

        [JsonProperty]
        public List<AttemptTimeInterval> TimeIntervals { get; private set; }

        [JsonProperty]
        public DateTimeOffset? Started { get; private set; }

        [JsonProperty]
        public DateTimeOffset? Pinged { get; private set; }

        [JsonProperty]
        public DateTimeOffset? Submitted { get; private set; }

        [JsonProperty]
        public decimal Duration { get; private set; }

        #region Methods (time intervals)

        private void OnAttemptStarted(Change e)
        {
            if (Started.HasValue)
                return;

            Started = e.ChangeTime;

            TimeIntervals = new List<AttemptTimeInterval>
            {
                new AttemptTimeInterval(e.ChangeTime, ActiveSectionIndex)
            };

            OnTimeIntervalChanged();
        }

        private void OnAttemptPinged(Change e)
        {
            if (Submitted.HasValue)
                return;

            var interval = TimeIntervals[TimeIntervals.Count - 1];
            var pingDate = interval.Pinged ?? interval.Started;

            if ((e.ChangeTime - pingDate).TotalSeconds > Configuration.PingInterval * 2)
            {
                OnAttemptResumed(e, Configuration.PingInterval);
                return;
            }

            interval.Pinged = Pinged = e.ChangeTime;

            if (ActiveSectionIndex.HasValue && ActiveSectionIndex.Value > interval.SectionIndex.Value)
            {
                interval.Ended = e.ChangeTime;
                TimeIntervals.Add(new AttemptTimeInterval(e.ChangeTime, ActiveSectionIndex));
            }

            OnTimeIntervalChanged();
        }

        private void OnAttemptResumed(Change e, int pingInterval)
        {
            if (Submitted.HasValue)
                return;

            var interval = TimeIntervals[TimeIntervals.Count - 1];
            interval.Ended = (interval.Pinged ?? interval.Started).AddSeconds(pingInterval);

            Pinged = e.ChangeTime;

            TimeIntervals.Add(new AttemptTimeInterval(e.ChangeTime, ActiveSectionIndex));

            OnTimeIntervalChanged();
        }

        private void OnAttemptSubmitted(Change e)
        {
            if (Submitted.HasValue)
                return;

            if (Started.HasValue)
            {
                var interval = TimeIntervals[TimeIntervals.Count - 1];
                var pingDate = interval.Pinged ?? interval.Started;

                if ((e.ChangeTime - pingDate).TotalSeconds > Configuration.PingInterval * 2)
                {
                    OnAttemptResumed(e, Configuration.PingInterval);
                    interval = TimeIntervals[TimeIntervals.Count - 1];
                }

                interval.Ended = e.ChangeTime;

                OnTimeIntervalChanged();
            }

            Submitted = e.ChangeTime;
        }

        private void OnTimeIntervalChanged()
        {
            if (Sections.IsNotEmpty())
            {
                var sectionsData = new IntervalCalculationItem[Sections.Length];

                for (var i = 0; i < TimeIntervals.Count; i++)
                {
                    var interval = TimeIntervals[i];
                    var sectionIndex = interval.SectionIndex.Value;
                    var section = sectionsData[sectionIndex]
                        ?? (sectionsData[sectionIndex] = new IntervalCalculationItem { Started = interval.Started });

                    if (interval.Ended.HasValue)
                        section.Ended = interval.Ended;

                    section.Duration += interval.Duration;
                }

                var totalDuration = 0d;

                for (var i = 0; i < Sections.Length; i++)
                {
                    var info = sectionsData[i] ?? new IntervalCalculationItem();
                    var section = Sections[i];

                    section.Started = info.Started;
                    section.Completed = info.Ended;

                    if (info.Duration > 0)
                    {
                        section.Duration = (int?)Math.Round(info.Duration, MidpointRounding.AwayFromZero);

                        if (!section.IsBreakTimer)
                            totalDuration += info.Duration;
                    }
                    else
                    {
                        section.Duration = null;
                    }
                }

                Duration = (decimal)totalDuration;
            }
            else
            {
                Duration = TimeIntervals.IsNotEmpty()
                    ? (decimal)TimeIntervals.Sum(x => x.Duration)
                    : 0;
            }
        }

        #endregion

        #region Methods (events)

        public void When(AttemptAnalyzed _)
        {

        }

        public void When(AttemptSubmitted e)
        {
            OnAttemptSubmitted(e);

            if (Sections.IsNotEmpty() && ActiveSectionIndex.HasValue)
                ActiveSectionIndex = Sections.Length;

            if (ActiveQuestionIndex.HasValue)
                ActiveQuestionIndex = Questions.Length;
        }

        public void When(AttemptGraded e)
        {
            if (!Submitted.HasValue)
                OnAttemptSubmitted(e);
        }

        public void When(AttemptGradedDateChanged _)
        {

        }

        public void When(AttemptFixed _)
        {

        }

        public void When(AttemptImported e)
        {
            Identifier = e.AggregateIdentifier;

            OnAttemptSubmitted(e);
        }

        public void When(AttemptPinged e)
        {
            OnAttemptPinged(e);
        }

        public void When(AttemptResumed e)
        {
            OnAttemptResumed(e, e.PingInterval ?? Configuration.PingInterval);
        }

        public void When(AttemptStarted2 e)
        {
            Identifier = e.AggregateIdentifier;
            Configuration = new AttemptConfiguration
            {
                TimeLimit = e.TimeLimit,
                Language = e.Language.IfNullOrEmpty(Language.Default),

                SectionsAsTabs = e.SectionsAsTabsEnabled,
                TabNavigation = e.TabNavigationEnabled,
                SingleQuestionPerTab = e.SingleQuestionPerTabEnabled,

                TabTimeLimit = SpecificationTabTimeLimit.Disabled
            };
            Sections = e.FormSectionsCount.HasValue
                ? Enumerable.Range(0, e.FormSectionsCount.Value).Select(x => new AttemptSectionState(x)).ToArray()
                : null;

            var questionIndexOffset = 0;

            Questions = e.Questions
                .Select((x, i) =>
                {
                    var result = new AttemptQuestionState(x, i + questionIndexOffset);

                    questionIndexOffset += result.SubQuestions.Length;

                    return result;
                })
                .ToArray();

            ActiveSectionIndex = e.ActiveSectionIndex;
            ActiveQuestionIndex = e.ActiveQuestionIndex;

            OnAttemptStarted(e);
        }

        public void When(AttemptStarted3 e)
        {
            Identifier = e.AggregateIdentifier;
            BankIdentifier = e.BankIdentifier;
            Configuration = e.Configuration.Clone();
            Sections = e.Sections.Select((x, i) => new AttemptSectionState(x, i)).ToArray();

            var questionIndexOffset = 0;

            Questions = e.Questions
                .Select((x, i) =>
                {
                    var result = new AttemptQuestionState(x, i + questionIndexOffset);

                    questionIndexOffset += result.SubQuestions.Length;

                    return result;
                })
                .ToArray();

            if (e.Configuration.SectionsAsTabs && !e.Configuration.TabNavigation)
            {
                ActiveSectionIndex = 0;
                if (e.Configuration.SingleQuestionPerTab)
                    ActiveQuestionIndex = 0;
            }

            OnAttemptStarted(e);
        }

        public void When(AttemptTagged _)
        {

        }

        public void When(AttemptVoided _)
        {

        }

        public void When(AttemptCommentPosted _)
        {

        }

        public void When(ComposedQuestionAnswered _)
        {

        }

        public void When(ComposedQuestionAttemptStarted _)
        {

        }

        public void When(ComposedQuestionScored _)
        {

        }

        public void When(MatchingQuestionAnswered _)
        {

        }

        public void When(MultipleChoiceQuestionAnswered _)
        {

        }

        public void When(MultipleCorrectQuestionAnswered _)
        {

        }

        public void When(QuestionVoided _)
        {

        }

        public void When(QuestionRegraded _)
        {

        }

        public void When(ScoreCalculated _)
        {

        }

        public void When(BooleanTableQuestionAnswered _)
        {

        }

        public void When(TrueOrFalseQuestionAnswered _)
        {

        }

        public void When(HotspotQuestionAnswered _)
        {

        }

        public void When(OrderingQuestionAnswered _)
        {

        }

        public void When(AttemptSectionSwitched e)
        {
            ActiveSectionIndex = e.NextSectionIndex;
            OnAttemptPinged(e);
        }

        public void When(AttemptQuestionSwitched e)
        {
            ActiveSectionIndex = e.NextSectionIndex;
            ActiveQuestionIndex = e.NextQuestionIndex;
            OnAttemptPinged(e);
        }

        public void When(AttemptGradingAssessorAssigned e)
        {

        }

        public void When(AttemptRubricPointsUpdated e)
        {
            var questionIds = new HashSet<Guid>(e.QuestionIds);

            foreach (var question in Questions.Where(x => questionIds.Contains(x.QuestionIdentifier)))
                question.Rubric.Points = e.RubricPoints;
        }

        public void When(AttemptRubricChanged e)
        {
            var questionIds = new HashSet<Guid>(e.QuestionIds);

            foreach (var question in Questions.Where(x => questionIds.Contains(x.QuestionIdentifier)))
            {
                var rubric = question.Rubric;
                rubric.Identifier = e.NewRubricId;
                rubric.Points = e.NewRubricPoints;
            }
        }

        public void When(AttemptQuestionRubricChanged e)
        {
            var question = Questions.FirstOrDefault(x => x.QuestionIdentifier == e.QuestionId);

            question.Rubric.Set(e.Rubric);
        }

        public void When(AttemptQuestionRubricInited e)
        {
            var question = Questions.FirstOrDefault(x => x.QuestionIdentifier == e.QuestionId);

            question.InitRubric(e.Rubric);
        }

        public void When(SerializedChange e)
        {
            // Obsolete changes go here

            if (e.ChangeType != ObsoleteChangeType.AttemptStarted)
                return;

            var v2 = AttemptStarted2.Upgrade(e);

            When(v2);
        }

        #endregion

        #region Classes

        private class IntervalCalculationItem
        {
            public DateTimeOffset? Started;
            public DateTimeOffset? Ended;
            public double Duration;
        }

        #endregion
    }
}
