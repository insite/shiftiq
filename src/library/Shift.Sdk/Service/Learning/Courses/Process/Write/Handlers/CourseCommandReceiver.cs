using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Read;
using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class CourseCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly ICourseStore _courseStore;

        public CourseCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository,
            ICourseStore courseStore
            )
        {
            _publisher = publisher;
            _repository = repository;
            _courseStore = courseStore;

            commander.Subscribe<RunCommands>(Handle);

            commander.Subscribe<CreateCourse>(Handle);
            commander.Subscribe<DeleteCourse>(Handle);
            commander.Subscribe<ModifyCourseTimestamps>(Handle);
            commander.Subscribe<ModifyCourseContent>(Handle);
            commander.Subscribe<AddCourseEnrollment>(Handle);
            commander.Subscribe<RemoveCourseEnrollment>(Handle);
            commander.Subscribe<CompleteCourseEnrollment>(Handle);
            commander.Subscribe<IncreaseCourseEnrollment>(Handle);
            commander.Subscribe<ModifyCourseEnrollment>(Handle);
            commander.Subscribe<ConnectCourseGradebook>(Handle);
            commander.Subscribe<ConnectCourseFramework>(Handle);
            commander.Subscribe<ConnectCourseMessage>(Handle);
            commander.Subscribe<ConfigureCourseCompletionActivity>(Handle);
            commander.Subscribe<ConnectCourseCatalog>(Handle);
            commander.Subscribe<ModifyCourseFieldText>(Handle);
            commander.Subscribe<ModifyCourseFieldDateTimeOffset>(Handle);
            commander.Subscribe<ModifyCourseFieldBool>(Handle);
            commander.Subscribe<ModifyCourseFieldInt>(Handle);
            commander.Subscribe<ModifyCourseFieldGuid>(Handle);
            commander.Subscribe<AddCourseUnit>(Handle);
            commander.Subscribe<RemoveCourseUnit>(Handle);
            commander.Subscribe<ModifyCourseUnitTimestamps>(Handle);
            commander.Subscribe<ModifyCourseUnitContent>(Handle);
            commander.Subscribe<RenameCourseUnit>(Handle);
            commander.Subscribe<ModifyCourseUnitCode>(Handle);
            commander.Subscribe<ModifyCourseUnitSequence>(Handle);
            commander.Subscribe<ModifyCourseUnitSource>(Handle);
            commander.Subscribe<ModifyCourseUnitAdaptive>(Handle);
            commander.Subscribe<ModifyCourseUnitPrerequisiteDeterminer>(Handle);
            commander.Subscribe<AddCourseUnitPrerequisite>(Handle);
            commander.Subscribe<RemoveCourseUnitPrerequisite>(Handle);
            commander.Subscribe<AddCourseModule>(Handle);
            commander.Subscribe<MoveCourseModule>(Handle);
            commander.Subscribe<RemoveCourseModule>(Handle);
            commander.Subscribe<ModifyCourseModuleTimestamps>(Handle);
            commander.Subscribe<ModifyCourseModuleContent>(Handle);
            commander.Subscribe<RenameCourseModule>(Handle);
            commander.Subscribe<ModifyCourseModuleCode>(Handle);
            commander.Subscribe<ModifyCourseModuleImage>(Handle);
            commander.Subscribe<ModifyCourseModuleSequence>(Handle);
            commander.Subscribe<ModifyCourseModuleSource>(Handle);
            commander.Subscribe<ModifyCourseModuleAdaptive>(Handle);
            commander.Subscribe<ModifyCourseModulePrerequisiteDeterminer>(Handle);
            commander.Subscribe<AddCourseModulePrerequisite>(Handle);
            commander.Subscribe<RemoveCourseModulePrerequisite>(Handle);
            commander.Subscribe<AddCourseActivity>(Handle);
            commander.Subscribe<MoveCourseActivity>(Handle);
            commander.Subscribe<ResequenceCourseActivities>(Handle);
            commander.Subscribe<RemoveCourseActivity>(Handle);
            commander.Subscribe<ModifyCourseActivityTimestamps>(Handle);
            commander.Subscribe<ModifyCourseActivityType>(Handle);
            commander.Subscribe<ModifyCourseActivityContent>(Handle);
            commander.Subscribe<ModifyCourseActivityUrl>(Handle);
            commander.Subscribe<AddCourseActivityCompetencies>(Handle);
            commander.Subscribe<RemoveCourseActivityCompetencies>(Handle);
            commander.Subscribe<ConnectCourseActivityAssessmentForm>(Handle);
            commander.Subscribe<ConnectCourseActivitySurveyForm>(Handle);
            commander.Subscribe<ConnectCourseActivityGradeItem>(Handle);
            commander.Subscribe<ConnectCourseActivityLegacyPrerequisite>(Handle);
            commander.Subscribe<ConnectCourseActivityQuiz>(Handle);
            commander.Subscribe<AddCourseActivityPrerequisite>(Handle);
            commander.Subscribe<RemoveCourseActivityPrerequisite>(Handle);
            commander.Subscribe<ModifyCourseActivityFieldText>(Handle);
            commander.Subscribe<ModifyCourseActivityFieldBool>(Handle);
            commander.Subscribe<ModifyCourseActivityFieldInt>(Handle);
            commander.Subscribe<ModifyCourseActivityFieldGuid>(Handle);
            commander.Subscribe<ModifyCourseActivityFieldDate>(Handle);
            commander.Subscribe<RemoveCourseEmptyNodes>(Handle);
        }

        private void Handle(RunCommands runCommands)
        {
            if (runCommands.Commands == null || runCommands.Commands.Length == 0)
                return;

            if (runCommands.Commands[0] is IHasAggregate create)
            {
                ((IHasRun)create).Run(null);
                RunCommands(create.Course, runCommands, 1);
            }
            else
            {
                _repository.LockAndRun<CourseAggregate>(runCommands.AggregateIdentifier, course =>
                {
                    RunCommands(course, runCommands, 0);
                });
            }
        }

        private void RunCommands(CourseAggregate course, RunCommands runCommands, int startIndex)
        {
            for (int i = startIndex; i < runCommands.Commands.Length; i++)
            {
                var command = runCommands.Commands[i];

                if (command.AggregateIdentifier != course.AggregateIdentifier)
                    throw new ArgumentException($"The command has wrong AggregateIdentifier: {command.AggregateIdentifier}");

                ((IHasRun)command).Run(course);
            }

            var transactionId = _courseStore.StartTransaction(course.AggregateIdentifier);
            try
            {
                Commit(course, runCommands, transactionId);
                _courseStore.CommitTransaction(transactionId);
            }
            catch
            {
                _courseStore.CancelTransaction(transactionId);
                throw;
            }
        }

        private void Handle<T>(T c) where T: Command, IHasRun
        {
            if (c is IHasAggregate create)
            {
                if (c.Run(null))
                    Commit(create.Course, c, null);

                return;
            }

            _repository.LockAndRun<CourseAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (c.Run(aggregate))
                    Commit(aggregate, c, null);
            });
        }

        private void Commit(CourseAggregate aggregate, ICommand c, Guid? changeTransactionId)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (Change change in changes)
            {
                change.AggregateState = aggregate.State;
                change.ChangeTransactionId = changeTransactionId;
                _publisher.Publish(change);
            }
        }
    }
}
