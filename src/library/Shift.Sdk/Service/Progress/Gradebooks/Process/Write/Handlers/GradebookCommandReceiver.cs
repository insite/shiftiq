using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Read;
using InSite.Application.Gradebooks;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Records.Write
{
    public class GradebookCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IEventSearch _eventSearch;
        private readonly IRecordSearch _recordSearch;

        public GradebookCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository,
            IEventSearch eventSearch, IRecordSearch recordSearch)
        {
            _publisher = publisher;
            _repository = repository;
            _eventSearch = eventSearch;
            _recordSearch = recordSearch;

            commander.Subscribe<AddEnrollment>(Handle);
            commander.Subscribe<AddGradebookValidation>(Handle);
            commander.Subscribe<AddGradeItem>(Handle);
            commander.Subscribe<CalculateGradebook>(Handle);
            commander.Subscribe<ChangeGradebookAchievement>(Handle);
            commander.Subscribe<AddGradebookEvent>(Handle);
            commander.Subscribe<RemoveGradebookEvent>(Handle);
            commander.Subscribe<ChangeGradebookPeriod>(Handle);
            commander.Subscribe<ChangeGradebookType>(Handle);
            commander.Subscribe<ChangeGradebookUserPeriod>(Handle);
            commander.Subscribe<ChangeGradebookValidation>(Handle);
            commander.Subscribe<ChangeGradeItem>(Handle);
            commander.Subscribe<ChangeGradeItemAchievement>(Handle);
            commander.Subscribe<ChangeGradeItemCalculation>(Handle);
            commander.Subscribe<ChangeGradeItemCompetencies>(Handle);
            commander.Subscribe<ChangeGradeItemHook>(Handle);
            commander.Subscribe<ChangeGradeItemMaxPoints>(Handle);
            commander.Subscribe<ChangeGradeItemNotifications>(Handle);
            commander.Subscribe<ChangeGradeItemPassPercent>(Handle);
            commander.Subscribe<CreateGradebook>(Handle);
            commander.Subscribe<DeleteEnrollment>(Handle);
            commander.Subscribe<DeleteGradebook>(Handle);
            commander.Subscribe<DeleteGradeItem>(Handle);
            commander.Subscribe<DuplicateGradebook>(Handle);
            commander.Subscribe<LockGradebook>(Handle);
            commander.Subscribe<NoteGradebookUser>(Handle);
            commander.Subscribe<ReferenceGradebook>(Handle);
            commander.Subscribe<ReferenceGradeItem>(Handle);
            commander.Subscribe<RenameGradebook>(Handle);
            commander.Subscribe<ReorderGradeItem>(Handle);
            commander.Subscribe<RestartEnrollment>(Handle);
            commander.Subscribe<UnlockGradebook>(Handle);
        }

        private void Commit(GradebookAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddGradeItem c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateItem(aggregate, c.Item, c.Code);

                aggregate.AddItem(c.Item, c.Code, c.Name, c.ShortName, c.IsReported, c.Format, c.Type, c.Weighting, c.PassPercent, c.Parent);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddEnrollment c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (gradebook) =>
            {
                if (gradebook.Data.IsLocked)
                    gradebook.AddWarning($"A command to add a learner enrollment has been sent to this locked gradebook. (The learner identifier is {c.Learner}.)");

                gradebook.AddEnrollment(c.Enrollment, c.Learner, c.Period, c.Time, c.Comment);
                Commit(gradebook, c);
            });
        }

        public void Handle(AddGradebookValidation c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                if (!aggregate.Data.ContainsLearner(c.User))
                    throw new MissingGradebookEnrollmentException(c.User);

                aggregate.AddValidation(c.User, c.Competency, c.Points);
                Commit(aggregate, c);
            });
        }

        public void Handle(CalculateGradebook c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                aggregate.CalculateRecord(c.Learners);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradebookAchievement c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var data = aggregate.Data;

                if (data.Achievement == c.Achievement)
                    return;

                aggregate.ChangeRecordAchievement(c.Achievement);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddGradebookEvent c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var data = aggregate.Data;

                if (data.Events.Contains(c.Event) && (!c.ReplacePrimary || data.PrimaryEvent == c.Event))
                    return;

                var isPrimary = c.ReplacePrimary || data.Events.Count == 0;

                if (c.ReplacePrimary && data.PrimaryEvent.HasValue && data.PrimaryEvent != c.Event)
                    aggregate.RemoveGradebookEvent(data.PrimaryEvent.Value, null);

                aggregate.AddGradebookEvent(c.Event, isPrimary);

                Commit(aggregate, c);
            });
        }

        public void Handle(RemoveGradebookEvent c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;
                if (!data.IsOpen)
                    return;

                ValidateLocked(aggregate);

                if (!data.Events.Contains(c.Event) && data.PrimaryEvent != c.Event)
                    return;

                var newPrimaryEvent = data.PrimaryEvent == c.Event && data.Events.Count > 1
                    ? data.Events.Where(x => x != c.Event).OrderBy(x => x).First()
                    : (Guid?)null;

                aggregate.RemoveGradebookEvent(c.Event, newPrimaryEvent);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradebookPeriod c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var data = aggregate.Data;

                if (data.Period == c.Period)
                    return;

                aggregate.ChangeGradebookPeriod(c.Period);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradebookType c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var data = aggregate.Data;
                var type = c.Type;
                var framework = c.Framework;

                if (data.Type == type && data.Framework == framework)
                    return;

                if (data.Enrollments.Count > 0 || data.RootItems.Count > 0)
                    return;

                if ((type == GradebookType.Standards || type == GradebookType.ScoresAndStandards) && !framework.HasValue)
                    throw new InvalidGradebookException("You must specify a framework when you enable standards on a gradebook.");

                aggregate.ChangeRecordType(type, framework);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradebookUserPeriod c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var user = aggregate.Data.Enrollments.Find(x => x.Learner == c.User);
                if (user == null || user.Period == c.Period)
                    return;

                aggregate.ChangeGradebookUserPeriod(c.User, c.Period);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradeItem c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var data = aggregate.Data;
                var item = c.Item;
                var code = c.Code;
                var parent = c.Parent;
                var i = data.FindItem(item);

                if (i == null)
                    throw new MissingGradeItemException(item);

                if (i.Code != code && data.ContainsCode(code))
                    throw new DuplicateGradeItemCodeException(code);

                if (parent.HasValue && i.Parent?.Identifier != parent && i.FindItem(parent.Value) != null)
                    throw new InvalidGradeItemParentException($"Child {parent} of item {item} cannot be its parent at the same time");

                aggregate.ChangeItem(c.Item, c.Code, c.Name, c.ShortName, c.IsReported, c.Format, c.Type, c.Weighting, c.Parent);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradeItemAchievement c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var i = aggregate.Data.FindItem(c.Item);

                if (i == null)
                    throw new MissingGradeItemException(c.Item);

                if (GradeItemAchievement.Equals(i.Achievement, c.Achievement))
                    return;

                aggregate.ChangeItemAchievement(c.Item, c.Achievement);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradeItemCalculation c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var data = aggregate.Data;

                if (!data.ContainsItem(c.Item))
                    return;

                foreach (var part in c.Parts)
                {
                    var i = data.FindItem(part.Item);
                    if (i == null)
                        throw new MissingGradeItemException(i.Identifier);
                }

                aggregate.AddParts(c.Item, c.Parts);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradeItemCompetencies c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var data = aggregate.Data;
                var i = data.FindItem(c.Item);

                if (i == null)
                    throw new MissingGradeItemException(c.Item);

                if (data.Type != GradebookType.ScoresAndStandards && data.Type != GradebookType.Standards)
                    throw new WrongGradebookTypeException("Gradebook does not have enabled Standards option");

                if ((i.Competencies == null) != (c.Competencies == null)
                    || i.Competencies != null
                        && (
                            i.Competencies.Length != c.Competencies.Length
                            || i.Competencies.Except(c.Competencies).Count() != 0
                        )
                    )
                {
                    aggregate.ChangeItemCompetencies(c.Item, c.Competencies);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGradeItemHook c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var i = aggregate.Data.FindItem(c.Item);

                if (i == null)
                    throw new MissingGradeItemException(c.Item);

                aggregate.ChangeItemHook(c.Item, c.Hook);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradeItemMaxPoints c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var i = aggregate.Data.FindItem(c.Item);

                if (i == null)
                    throw new MissingGradeItemException(c.Item);

                aggregate.ChangeItemMaxPoints(c.Item, c.MaxPoints);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradeItemNotifications c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);
                var i = aggregate.Data.FindItem(c.Item);
                if (i == null)
                    throw new MissingGradeItemException(c.Item);
                aggregate.ChangeGradeItemNotifications(c.Item, c.Notifications);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeGradeItemPassPercent c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var i = aggregate.Data.FindItem(c.Item);

                if (i == null)
                    throw new MissingGradeItemException(c.Item);

                if (i.PassPercent != c.PassPercent)
                {
                    aggregate.ChangeItemPassPercent(c.Item, c.PassPercent);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ChangeGradebookValidation c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                if (!aggregate.Data.ContainsLearner(c.User))
                    throw new MissingGradebookEnrollmentException(c.User);

                aggregate.ChangeValidation(c.User, c.Competency, c.Points);
                Commit(aggregate, c);
            });
        }

        public void Handle(CreateGradebook c)
        {
            if ((c.Type == GradebookType.Standards || c.Type == GradebookType.ScoresAndStandards) && !c.Framework.HasValue)
                throw new InvalidGradebookException("You must specify a framework when you enable standards on a record.");

            var aggregate = new GradebookAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.CreateRecord(c.Tenant, c.Name, c.Type, c.Event, c.Achievement, c.Framework);
            Commit(aggregate, c);
        }

        public void Handle(DeleteGradeItem c)
        {
            if (_recordSearch.ItemHasProgress(c.AggregateIdentifier, c.Item))
                throw new DeleteGradeItemException($"Item {c.Item} has associated progresses and cannot be deleted");

            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var i = aggregate.Data.FindItem(c.Item);
                if (i == null)
                    return;

                if (i.Children.IsNotEmpty())
                    throw new DeleteGradeItemException($"Item {c.Item} has children and cannot be deleted");

                aggregate.DeleteItem(c.Item);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteEnrollment c)
        {
            var record = _repository.GetClone<GradebookAggregate>(c.AggregateIdentifier);

            ValidateLocked(record);

            if (!record.Data.ContainsLearner(c.Learner))
                return;

            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.DeleteUser(c.Learner);
                Commit(aggregate, c);
            });
        }

        public void Handle(DuplicateGradebook c)
        {
            var source = _repository.GetClone<GradebookAggregate>(c.Source).Data;
            var destination = new GradebookAggregate { AggregateIdentifier = c.AggregateIdentifier };

            var @event = c.Event.HasValue
                ? _eventSearch.GetEvent(c.Event.Value)
                : null;

            destination.CreateRecord(c.Tenant, c.Name, c.Type, c.Event, c.Achievement, c.Framework);

            var mapping = new Dictionary<Guid, Guid>();

            CopyItems(destination, source.RootItems, null, @event?.EventScheduledEnd, mapping);
            CopyParts(destination, source.RootItems, mapping);

            Commit(destination, c);
        }

        public void Handle(LockGradebook c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Lock();
                Commit(aggregate, c);
            });
        }

        public void Handle(NoteGradebookUser c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                if (!aggregate.Data.ContainsLearner(c.User))
                    return;

                aggregate.NoteUser(c.User, c.Note, c.Added);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReferenceGradebook c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                aggregate.ReferenceRecord(c.Reference);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReferenceGradeItem c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var i = aggregate.Data.FindItem(c.Item);
                if (i == null)
                    return;

                aggregate.ReferenceItem(c.Item, c.Reference);
                Commit(aggregate, c);
            });
        }

        public void Handle(RestartEnrollment c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                aggregate.RestartEnrollment(c.Learner, c.Time);
                Commit(aggregate, c);
            });
        }

        public void Handle(RenameGradebook c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                if (aggregate.Data.Name != c.Name)
                {
                    aggregate.RenameRecord(c.Name);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ReorderGradeItem c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                ValidateLocked(aggregate);

                var items = c.Parent.HasValue ? aggregate.Data.FindItem(c.Parent.Value)?.Children : aggregate.Data.RootItems;

                if (items == null
                    || items.Count != c.Children.Length
                    || items.Count(x => c.Children.Contains(x.Identifier)) != c.Children.Length
                    )
                {
                    return;
                }

                aggregate.ReorderItem(c.Parent, c.Children);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnlockGradebook c)
        {
            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.IsLocked)
                {
                    aggregate.Unlock();
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(DeleteGradebook c)
        {
            if (_recordSearch.RecordHasProgress(c.AggregateIdentifier))
                throw new CloseGradebookException($"Record has progress and cannot be deleted");

            _repository.LockAndRun<GradebookAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var data = aggregate.Data;

                if (data.Enrollments.Count > 0)
                    throw new CloseGradebookException($"Gradebook has students and cannot be voided");

                aggregate.Delete();
                Commit(aggregate, c);
            });
        }

        private void CopyItems(
            GradebookAggregate destination,
            List<GradeItem> items,
            Guid? parent,
            DateTimeOffset? achievementFixedDate,
            Dictionary<Guid, Guid> mapping
            )
        {
            foreach (var item in items)
            {
                var identifier = UuidFactory.Create();

                mapping.Add(item.Identifier, identifier);

                destination.AddItem(identifier, item.Code, item.Name, item.ShortName, item.IsReported, item.Format, item.Type, item.Weighting, item.PassPercent, parent);

                if (item.PassPercent.HasValue)
                    destination.ChangeItemPassPercent(identifier, item.PassPercent);

                if (item.Achievement != null)
                {
                    var achievement = item.Achievement.Clone();
                    achievement.AchievementFixedDate = achievementFixedDate;

                    destination.ChangeItemAchievement(identifier, achievement);
                }

                if (item.Children != null)
                    CopyItems(destination, item.Children, identifier, achievementFixedDate, mapping);
            }
        }

        private void CopyParts(GradebookAggregate destination, List<GradeItem> items, Dictionary<Guid, Guid> mapping)
        {
            foreach (var item in items)
            {
                if (item.Type == GradeItemType.Calculation && item.Parts.IsNotEmpty())
                {
                    var parts = new CalculationPart[item.Parts.Length];

                    for (int i = 0; i < item.Parts.Length; i++)
                    {
                        var source = item.Parts[i];

                        parts[i] = new CalculationPart
                        {
                            Item = mapping[source.Item],
                            Score = source.Score
                        };
                    }

                    destination.AddParts(mapping[item.Identifier], parts);
                }

                if (item.Children != null)
                    CopyParts(destination, item.Children, mapping);
            }
        }

        private void ValidateLocked(GradebookAggregate record)
        {
            if (record.Data.IsLocked)
                throw new LockedGradebookException();
        }

        private void ValidateItem(GradebookAggregate record, Guid item, string code)
        {
            var data = record.Data;

            if (data.IsLocked)
                throw new LockedGradebookException();

            if (!string.IsNullOrEmpty(code) && data.ContainsCode(code))
                throw new DuplicateGradeItemCodeException(code);

            if (data.ContainsItem(item))
                throw new DuplicateGradeItemKeyException(item);
        }
    }
}