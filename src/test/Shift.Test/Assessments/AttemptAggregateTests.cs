using InSite.Domain.Attempts;

using Shift.Common;
using Shift.Constant;

namespace Shift.Test.Assessments
{
    public class AttemptAggregateTests
    {
        [Fact]
        public void SwitchAttemptQuestion_ForcedPastLastQuestionInLastSection_DoesNothing()
        {
            var aggregate = StartAttempt(
                new[] { CreateSection(false), CreateSection(false) },
                new[] { 0, 1, 1 });

            aggregate.SwitchAttemptQuestion(1, false);

            Assert.Equal(1, aggregate.Data.ActiveSectionIndex);
            Assert.Equal(1, aggregate.Data.ActiveQuestionIndex);

            var changeCount = aggregate.GetUncommittedChanges().Length;

            aggregate.SwitchAttemptQuestion(aggregate.Data.Questions.Length, true);

            Assert.Equal(1, aggregate.Data.ActiveSectionIndex);
            Assert.Equal(1, aggregate.Data.ActiveQuestionIndex);
            Assert.Equal(changeCount, aggregate.GetUncommittedChanges().Length);

            aggregate.SubmitAttempt(null, true);

            Assert.NotNull(aggregate.Data.Submitted);
        }

        [Fact]
        public void SwitchAttemptQuestion_ForcedPastLastQuestionBeforeFinalBreak_SwitchesToBreak()
        {
            var aggregate = StartAttempt(
                new[] { CreateSection(false), CreateSection(true) },
                new[] { 0, 0 });

            aggregate.SwitchAttemptQuestion(aggregate.Data.Questions.Length, true);

            Assert.Equal(1, aggregate.Data.ActiveSectionIndex);
            Assert.Equal(1, aggregate.Data.ActiveQuestionIndex);
        }

        private static AttemptAggregate StartAttempt(AttemptSection[] sections, int[] questionSections)
        {
            var aggregate = new AttemptAggregate { AggregateIdentifier = Guid.NewGuid() };

            var config = new AttemptConfiguration
            {
                SectionsAsTabs = true,
                SingleQuestionPerTab = true,
                TabTimeLimit = SpecificationTabTimeLimit.AllTabs
            };

            var questions = questionSections
                .Select(section => (AttemptQuestion)new AttemptQuestionDefault
                {
                    Identifier = Guid.NewGuid(),
                    Section = section,
                    Type = QuestionItemType.SingleCorrect,
                    Options = new AttemptOption[0]
                })
                .ToArray();

            aggregate.StartAttempt(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                null, null, config, sections, questions);

            return aggregate;
        }

        private static AttemptSection CreateSection(bool isBreak) => new AttemptSection
        {
            Identifier = Guid.NewGuid(),
            IsBreakTimer = isBreak,
            TimeLimit = 60,
            TimerType = FormSectionTimeType.Optional
        };
    }
}
