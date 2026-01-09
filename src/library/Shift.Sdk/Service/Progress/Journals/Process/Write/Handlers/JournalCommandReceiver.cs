using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Journals.Write;
using InSite.Domain.Records;

namespace InSite.Application.Records.Write
{
    public class JournalCommandReceiver
    {
        private readonly ICommandQueue _commander;
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public JournalCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _commander = commander;
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AddComment>(Handle);
            commander.Subscribe<ChangeComment>(Handle);
            commander.Subscribe<DeleteComment>(Handle);
            commander.Subscribe<AddExperience>(Handle);
            commander.Subscribe<AddExperienceCompetency>(Handle);
            commander.Subscribe<ChangeExperienceCompetency>(Handle);
            commander.Subscribe<ChangeExperienceCompetencySatisfactionLevel>(Handle);
            commander.Subscribe<ChangeExperienceCompetencySkillRating>(Handle);
            commander.Subscribe<DeleteExperienceCompetency>(Handle);
            commander.Subscribe<DeleteExperience>(Handle);
            commander.Subscribe<ChangeExperienceCompleted>(Handle);
            commander.Subscribe<ChangeExperienceEmployer>(Handle);
            commander.Subscribe<ChangeExperienceEvidence>(Handle);
            commander.Subscribe<ChangeExperienceHours>(Handle);
            commander.Subscribe<ChangeExperienceInstructor>(Handle);
            commander.Subscribe<ChangeExperienceSupervisor>(Handle);
            commander.Subscribe<ChangeExperienceTime>(Handle);
            commander.Subscribe<ChangeExperienceTraining>(Handle);
            commander.Subscribe<ChangeExperienceMediaEvidence>(Handle);
            commander.Subscribe<ValidateExperience>(Handle);
            commander.Subscribe<CreateJournal>(Handle);
            commander.Subscribe<DeleteJournal>(Handle);
        }

        private void Commit(JournalAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddComment c)
        {
            _repository.LockAndRun<JournalAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.FindComment(c.Comment) != null)
                    throw new JournalException($"Comment {c.Comment} is already exist in journal {c.AggregateIdentifier}");

                aggregate.AddComment(c.Comment, c.Author, c.Subject, c.SubjectType, c.Text, c.Posted, c.IsPrivate);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeComment c)
        {
            _repository.LockAndRun<JournalAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var comment = aggregate.Data.FindComment(c.Comment);

                if (comment == null)
                    throw new JournalException($"Comment {c.Comment} is not found in journal {c.AggregateIdentifier}");

                if (comment.Text != c.Text || comment.IsPrivate != c.IsPrivate)
                {
                    aggregate.ChangeComment(c.Comment, c.Text, c.Revised, c.IsPrivate);

                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(DeleteComment c)
        {
            _repository.LockAndRun<JournalAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.FindComment(c.Comment) != null)
                {
                    aggregate.DeleteComment(c.Comment);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(AddExperience c)
        {
            _repository.LockAndRun<JournalAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                if (aggregate.Data.FindExperience(c.Experience) != null)
                    throw new JournalException($"Experience {c.Experience} is already exist in journal {c.AggregateIdentifier}");

                aggregate.AddExperience(c.Experience);

                Commit(aggregate, c);
            });
        }

        public void Handle(AddExperienceCompetency c)
        {
            _repository.LockAndRun<JournalAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var experience = aggregate.Data.FindExperience(c.Experience);
                if (experience == null)
                    throw new JournalException($"Experience {c.Experience} is not found in journal {c.AggregateIdentifier}");

                if (experience.FindExperienceCompetency(c.Competency) != null)
                    throw new JournalException($"Competency {c.Competency} is already exist in experience {c.Experience}");

                aggregate.AddExperienceCompetency(c.Experience, c.Competency, c.Hours);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceCompetency c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                var competency = experience.FindExperienceCompetency(c.Competency);
                if (competency == null)
                    throw new JournalException($"Competency {c.Competency} is not found in experience {c.Experience}");

                if (competency.Hours == c.Hours)
                    return;

                aggregate.ChangeExperienceCompetency(c.Experience, c.Competency, c.Hours);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceCompetencySatisfactionLevel c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                var competency = experience.FindExperienceCompetency(c.Competency);
                if (competency == null)
                    throw new JournalException($"Competency {c.Competency} is not found in experience {c.Experience}");

                if (competency.SatisfactionLevel == c.SatisfactionLevel)
                    return;

                aggregate.ChangeExperienceCompetencySatisfactionLevel(c.Experience, c.Competency, c.SatisfactionLevel);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceCompetencySkillRating c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                var competency = experience.FindExperienceCompetency(c.Competency);
                if (competency == null)
                    throw new JournalException($"Competency {c.Competency} is not found in experience {c.Experience}");

                if (competency.SkillRating == c.SkillRating)
                    return;

                aggregate.ChangeExperienceCompetencySkillRating(c.Experience, c.Competency, c.SkillRating);

                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteExperienceCompetency c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                var competency = experience.FindExperienceCompetency(c.Competency);
                if (competency == null)
                    throw new JournalException($"Competency {c.Competency} is not found in experience {c.Experience}");

                aggregate.DeleteExperienceCompetency(c.Experience, c.Competency);

                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteExperience c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                aggregate.DeleteExperience(c.Experience);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceCompleted c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (experience.Completed == c.Completed)
                    return;

                aggregate.ChangeExperienceCompleted(c.Experience, c.Completed);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceEmployer c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (string.Equals(experience.Employer, c.Employer))
                    return;

                aggregate.ChangeExperienceEmployer(c.Experience, c.Employer);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceEvidence c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (string.Equals(experience.Evidence, c.Evidence))
                    return;

                aggregate.ChangeExperienceEvidence(c.Experience, c.Evidence);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceHours c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (experience.Hours == c.Hours)
                    return;

                aggregate.ChangeExperienceHours(c.Experience, c.Hours);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceInstructor c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (string.Equals(experience.Instructor, c.Instructor))
                    return;

                aggregate.ChangeExperienceInstructor(c.Experience, c.Instructor);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceSupervisor c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (string.Equals(experience.Supervisor, c.Supervisor))
                    return;

                aggregate.ChangeExperienceSupervisor(c.Experience, c.Supervisor);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceTime c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (experience.Started == c.Started && experience.Stopped == c.Stopped)
                    return;

                aggregate.ChangeExperienceTime(c.Experience, c.Started, c.Stopped);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceTraining c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (string.Equals(experience.TrainingLevel, c.Level)
                    && string.Equals(experience.TrainingLocation, c.Location)
                    && string.Equals(experience.TrainingProvider, c.Provider)
                    && string.Equals(experience.TrainingCourseTitle, c.CourseTitle)
                    && string.Equals(experience.TrainingComment, c.Comment)
                    && string.Equals(experience.TrainingType, c.Type)
                    )
                {
                    return;
                }

                aggregate.ChangeExperienceTraining(c.Experience, c.Level, c.Location, c.Provider, c.CourseTitle, c.Comment, c.Type);

                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeExperienceMediaEvidence c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                aggregate.ChangeExperienceMediaEvidence(c.Experience, c.Name, c.Type, c.FileIdentifier);

                Commit(aggregate, c);
            });
        }

        public void Handle(ValidateExperience c)
        {
            ChangeExperience(c, c.Experience, (aggregate, experience) =>
            {
                if (experience.Validator == c.Validator
                    && experience.Validated == c.Validated
                    && experience.SkillRating == c.SkillRating
                    )
                {
                    return;
                }

                aggregate.ValidateExperience(c.Experience, c.Validator, c.Validated, c.SkillRating);

                Commit(aggregate, c);
            });
        }

        public void Handle(CreateJournal c)
        {
            var aggregate = new JournalAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.JournalSetup };

            aggregate.Create(c.JournalSetup, c.User);

            Commit(aggregate, c);
        }

        public void Handle(DeleteJournal c)
        {
            _repository.LockAndRun<JournalAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.Delete();

                Commit(aggregate, c);
            });
        }

        private void ChangeExperience(Command c, Guid experienceIdentifier, Action<JournalAggregate, Experience> action)
        {
            _repository.LockAndRun<JournalAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                var experience = aggregate.Data.FindExperience(experienceIdentifier);
                if (experience == null)
                    throw new JournalException($"Experience {experienceIdentifier} is not found in journal {c.AggregateIdentifier}");

                action(aggregate, experience);
            });
        }
    }
}
