using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

namespace InSite.Application.Records.Write
{
    public class ProgressCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IRecordSearch _records;

        public ProgressCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository, IRecordSearch records)
        {
            _publisher = publisher;
            _repository = repository;
            _records = records;

            commander.Subscribe<AddProgress>(Handle);
            commander.Subscribe<ChangeProgressComment>(Handle);
            commander.Subscribe<ChangeProgressNumber>(Handle);
            commander.Subscribe<ChangeProgressPercent>(Handle);
            commander.Subscribe<ChangeProgressPoints>(Handle);
            commander.Subscribe<ChangeProgressText>(Handle);
            commander.Subscribe<CompleteProgress>(Handle);
            commander.Subscribe<DeleteProgress>(Handle);
            commander.Subscribe<HideProgress>(Handle);
            commander.Subscribe<IncompleteProgress>(Handle);
            commander.Subscribe<LockProgress>(Handle);
            commander.Subscribe<PublishProgress>(Handle);
            commander.Subscribe<ShowProgress>(Handle);
            commander.Subscribe<StartProgress>(Handle);
            commander.Subscribe<UnlockProgress>(Handle);
            commander.Subscribe<IgnoreProgress>(Handle);
        }

        private void Commit(ProgressAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddProgress c)
        {
            var gradebook = _records.GetGradebook(c.Gradebook);
            if (gradebook == null)
                throw new MissingGradebookException(c.Gradebook);

            if (gradebook.IsLocked)
                throw new LockedGradebookException($"Record {c.Gradebook} is locked");

            var gradeItem = _records.GetGradeItem(c.GradeItem);

            if (gradeItem == null || gradeItem.GradebookIdentifier != c.Gradebook)
                throw new MissingGradeItemException(c.GradeItem);

            if (!_records.EnrollmentExists(c.Gradebook, c.Learner))
                throw new MissingGradebookEnrollmentException(c.Learner);

            if (_records.ProgressExists(c.Gradebook, c.GradeItem, c.Learner))
                return;

            var aggregate = !_repository.Exists<ProgressAggregate>(c.AggregateIdentifier)
                ? new ProgressAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.Gradebook }
                : _repository.Get<ProgressAggregate>(c.AggregateIdentifier);

            aggregate.AddProgress(c.Gradebook, c.Learner, c.GradeItem);

            Commit(aggregate, c);
        }

        public void Handle(ChangeProgressComment c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Comment != c.Comment)
                {
                    aggregate.ChangeComment(c.Comment);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeProgressNumber c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Number != c.Number || aggregate.Data.Graded != c.Graded)
                {
                    aggregate.ChangeNumber(c.Number, c.Graded);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeProgressPercent c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Percent != c.Percent || aggregate.Data.Graded != c.Graded)
                {
                    aggregate.ChangePercent(c.Percent, c.Graded);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeProgressPoints c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Points != c.Points || aggregate.Data.MaxPoints != c.MaxPoints || aggregate.Data.Graded != c.Graded)
                {
                    aggregate.ChangePoints(c.Points, c.MaxPoints, c.Graded);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeProgressText c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.Text != c.Text || aggregate.Data.Graded != c.Graded)
                {
                    aggregate.ChangeText(c.Text, c.Graded);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(CompleteProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Complete(c.Completed, c.Percent, c.Pass, c.ElapsedMinutes, c.ElapsedSeconds);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Delete();
                Commit(aggregate, c);
            });
        }

        public void Handle(HideProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Hide();
                Commit(aggregate, c);
            });
        }

        public void Handle(IncompleteProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Incomplete();
                Commit(aggregate, c);
            });
        }

        public void Handle(LockProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Lock();
                Commit(aggregate, c);
            });
        }

        public void Handle(PublishProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Publish();
                Commit(aggregate, c);
            });
        }

        public void Handle(ShowProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Show();
                Commit(aggregate, c);
            });
        }

        public void Handle(StartProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Start(c.Started);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnlockProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Unlock();
                Commit(aggregate, c);
            });
        }

        public void Handle(IgnoreProgress c)
        {
            ValidateProgress(c.AggregateIdentifier);

            _repository.LockAndRun<ProgressAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Ignore(c.IsIgnored);
                Commit(aggregate, c);
            });
        }

        private void ValidateProgress(Guid progressIdentifier)
        {
            var progress = _repository.GetClone<ProgressAggregate>(progressIdentifier);
            if (progress == null)
                throw new ProgressNotFoundException($"Progress {progressIdentifier} does not exist");

            var record = _repository.GetClone<GradebookAggregate>(progress.Data.Record);
            if (record.Data.IsLocked)
                throw new LockedGradebookException();
        }
    }
}