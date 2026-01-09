using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Domain.Attempts;

using Shift.Common;

namespace InSite.Application.Attempts.Write
{
    public class AttemptCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public AttemptCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AnalyzeAttempt>(Handle);
            commander.Subscribe<AnswerComposedQuestion>(Handle);
            commander.Subscribe<AnswerMatchingQuestion>(Handle);
            commander.Subscribe<AnswerMultipleChoiceQuestion>(Handle);
            commander.Subscribe<AnswerMultipleCorrectQuestion>(Handle);
            commander.Subscribe<AnswerBooleanTableQuestion>(Handle);
            commander.Subscribe<AnswerTrueOrFalseQuestion>(Handle);
            commander.Subscribe<AnswerHotspotQuestion>(Handle);
            commander.Subscribe<AnswerOrderingQuestion>(Handle);
            commander.Subscribe<AuthorComment>(Handle);
            commander.Subscribe<CalculateScore>(Handle);
            commander.Subscribe<SubmitAttempt>(Handle);
            commander.Subscribe<GradeAttempt>(Handle);
            commander.Subscribe<FixAttempt>(Handle);
            commander.Subscribe<ImportAttempt>(Handle);
            commander.Subscribe<PingAttempt>(Handle);
            commander.Subscribe<ResumeAttempt>(Handle);
            commander.Subscribe<ScoreComposedQuestion>(Handle);
            commander.Subscribe<StartAttempt>(Handle);
            commander.Subscribe<StartComposedQuestionAttempt>(Handle);
            commander.Subscribe<TagAttempt>(Handle);
            commander.Subscribe<VoidAttempt>(Handle);
            commander.Subscribe<VoidQuestion>(Handle);
            commander.Subscribe<RegradeQuestion>(Handle);
            commander.Subscribe<ChangeAttempGradedDate>(Handle);
            commander.Subscribe<SwitchAttemptSection>(Handle);
            commander.Subscribe<SwitchAttemptQuestion>(Handle);
            commander.Subscribe<ChangeAttempGradingAssessor>(Handle);
            commander.Subscribe<UpdateAttemptRubricPoints>(Handle);
            commander.Subscribe<ChangeAttemptQuestionRubric>(Handle);
            commander.Subscribe<InitAttemptQuestionRubric>(Handle);
        }

        private void Commit(AttemptAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AnalyzeAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnalyzeAttempt(c.AllowTakeAttendance);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerComposedQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerComposedQuestion(c.Question, c.Answer);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerMatchingQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerMatchingQuestion(c.Question, c.Matches);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerMultipleChoiceQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerMultipleChoiceQuestion(c.Question, c.Option);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerMultipleCorrectQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerMultipleCorrectQuestion(c.Question, c.Options);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerTrueOrFalseQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerTrueOrFalseQuestion(c.Question, c.Option);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerBooleanTableQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerBooleanTableQuestion(c.Question, c.Options);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerHotspotQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerHotspotQuestion(c.Question, c.Pins);
                Commit(aggregate, c);
            });
        }

        public void Handle(AnswerOrderingQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AnswerOrderingQuestion(c.Question, c.OptionsOrder);
                Commit(aggregate, c);
            });
        }

        public void Handle(AuthorComment c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AuthorComment(c.Question, c.Comment);
                Commit(aggregate, c);
            });
        }

        public void Handle(CalculateScore c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.CalculateScore(c.Points, c.Score, c.Grade, c.IsPassing);
                Commit(aggregate, c);
            });
        }

        public void Handle(SubmitAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.SubmitAttempt(c.UserAgent, c.Grade);
                Commit(aggregate, c);
            });
        }

        public void Handle(GradeAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.GradeAttempt();
                Commit(aggregate, c);
            });
        }

        public void Handle(FixAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.FixAttempt(c.Points, c.Score, c.IsPassing, c.Registration);
                Commit(aggregate, c);
            });
        }

        public void Handle(ImportAttempt c)
        {
            var aggregate = new AttemptAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.Bank };
            aggregate.ImportAttempt(c.Tenant, c.Answers, c.Started, c.Completed, c.Tag, c.Form, c.Candidate, c.Registration, c.IsAttended, c.Language);
            Commit(aggregate, c);
        }

        public void Handle(PingAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.PingAttempt();
                Commit(aggregate, c);
            });
        }

        public void Handle(ResumeAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ResumeAttempt(c.PingInterval);
                Commit(aggregate, c);
            });
        }

        public void Handle(ScoreComposedQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ScoreComposedQuestion(c.Question, c.RubricRatingPoints);
                Commit(aggregate, c);
            });
        }

        public void Handle(StartAttempt c)
        {
            var aggregate = new AttemptAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.BankIdentifier };
            aggregate.StartAttempt(c.OrganizationIdentifier, c.BankIdentifier, c.FormIdentifier, c.AssessorUserIdentifier, c.LearnerUserIdentifier, c.RegistrationIdentifier, c.UserAgent, c.Configuration, c.Sections.EmptyIfNull(), c.Questions.EmptyIfNull());
            Commit(aggregate, c);
        }

        public void Handle(StartComposedQuestionAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.StartComposedQuestionAttempt(c.Question);
                Commit(aggregate, c);
            });
        }

        public void Handle(TagAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.TagAttempt(c.Tag);
                Commit(aggregate, c);
            });
        }

        public void Handle(VoidAttempt c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.VoidAttempt(c.Reason);
                Commit(aggregate, c);
            });
        }

        public void Handle(VoidQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.VoidQuestion(c.Question);
                Commit(aggregate, c);
            });
        }

        public void Handle(RegradeQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RegradeQuestion(c.Form, c.Question, c.OldOptions, c.RegradeOption);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeAttempGradedDate c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeAttemptCompleteDate(c.Completed);
                Commit(aggregate, c);
            });
        }

        public void Handle(SwitchAttemptSection c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.SwitchAttemptSection(c.NextSectionIndex);
                Commit(aggregate, c);
            });
        }

        public void Handle(SwitchAttemptQuestion c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.SwitchAttemptQuestion(c.NextQuestionIndex);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeAttempGradingAssessor c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeAttemptGradingAssessor(c.GradingAssessor);
                Commit(aggregate, c);
            });
        }

        public void Handle(UpdateAttemptRubricPoints c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UpdateAttemptRubricPoints(c.RubricId, c.RubricPoints);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeAttemptQuestionRubric c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeAttemptQuestionRubric(c.QuestionId, c.Rubric);
                Commit(aggregate, c);
            });
        }

        public void Handle(InitAttemptQuestionRubric c)
        {
            _repository.LockAndRun<AttemptAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.InitAttemptQuestionRubric(c.QuestionId, c.Rubric);
                Commit(aggregate, c);
            });
        }
    }
}