using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Credentials.Write;
using InSite.Application.Logs.Read;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Write;
using InSite.Domain.Messages;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;
using Shift.Constant;

namespace InSite.Application.Records.Read
{
    /// <summary>
    /// Implements the process manager for Gradebook changes. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain changes in 
    /// a cross-aggregate, eventually-consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming changes (which may be published from many aggregates). Some states 
    /// will have side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class GradebookChangeProcessor
    {
        private readonly ICommander _commander;

        private readonly IAlertMailer _mailer;
        private readonly Urls _urls;

        private readonly IAchievementSearch _achievements;
        private readonly IBankSearch _banks;
        private readonly IRecordSearch _records;
        private readonly ICourseObjectSearch _courses;
        private readonly ICourseObjectStore _courseStore;
        private readonly IMessageSearch _messages;
        private readonly IContactSearch _contacts;
        private readonly IContentSearch _contents;
        private readonly IProgramSearch _program;
        private readonly IProgramStore _programStore;
        private readonly IProgramService _programService;
        private readonly IAggregateSearch _aggregateSearch;

        public GradebookChangeProcessor(
            ICommander commander,
            IChangeQueue publisher,
            IAlertMailer mailer,
            Urls urls,
            IAchievementSearch achievements,
            IBankSearch banks,
            IRecordSearch records,
            ICourseObjectSearch courses,
            ICourseObjectStore courseStore,
            IMessageSearch messages,
            IContactSearch contacts,
            IContentSearch contents,
            IProgramSearch program,
            IProgramStore programStore,
            IProgramService programService,
            IAggregateSearch aggregateSearch
            )
        {
            _commander = commander;
            _mailer = mailer;
            _urls = urls;

            _achievements = achievements;
            _banks = banks;
            _records = records;
            _courses = courses;
            _courseStore = courseStore;
            _messages = messages;
            _contacts = contacts;
            _contents = contents;
            _program = program;
            _programStore = programStore;
            _programService = programService;
            _aggregateSearch = aggregateSearch;

            publisher.Subscribe<GradebookUserDeleted>(Handle);

            publisher.Subscribe<GradeItemPassPercentChanged>(Handle);

            publisher.Subscribe<ProgressStarted>(Handle);
            publisher.Subscribe<ProgressPercentChanged>(Handle);
            publisher.Subscribe<ProgressCompleted2>(Handle);
            publisher.Subscribe<ProgressPublished>(Handle);
            publisher.Subscribe<ProgressDeleted>(Handle);
        }

        private void Handle(GradebookUserDeleted c)
        {
            var userProgresses = _records.GetGradebookScores(new QProgressFilter { GradebookIdentifier = c.AggregateIdentifier, StudentUserIdentifier = c.User });
            foreach (var progress in userProgresses)
                _commander.Send(new DeleteProgress(progress.ProgressIdentifier));
        }

        private void Handle(GradeItemPassPercentChanged e)
        {
            var specs = GetBankSpecificationsForRelatedGradeItems(e.Item);
            foreach (var spec in specs)
                if (e.PassPercent != spec.CalcPassingScore)
                    _commander.Send(new Banks.Write.ChangeSpecificationCalculation(spec.BankIdentifier, spec.SpecIdentifier, CreateBankSpecificationCalculation(spec.CalcDisclosure, e.PassPercent)));
        }

        private QBankSpecification[] GetBankSpecificationsForRelatedGradeItems(Guid gradeitem)
        {
            var list = new List<QBankSpecification>();
            var forms = _banks.GetForms(new QBankFormFilter { GradeItemIdentifier = gradeitem });
            var identifiers = forms.Select(x => x.SpecIdentifier).Distinct().ToArray();
            foreach (var id in identifiers)
            {
                var spec = _banks.GetSpecification(id);
                if (spec != null)
                    list.Add(spec);
            }
            return list.ToArray();
        }

        private Domain.Banks.ScoreCalculation CreateBankSpecificationCalculation(string disclosure, decimal? pass)
        {
            return new Domain.Banks.ScoreCalculation
            {
                Disclosure = disclosure.ToEnum<DisclosureType>(),
                PassingScore = pass ?? 0m
            };
        }

        private void Handle(ProgressPercentChanged e)
            => HandleItemAchievement(e, TriggerCauseChange.Changed, e.Percent);

        private void Handle(ProgressCompleted2 e)
        {
            HandleItemAchievement(e, TriggerCauseChange.Changed, e.Percent);
            HandleCompletedCourse(e);
        }

        private void Handle(ProgressPublished e)
        {
            HandleItemAchievement(e, TriggerCauseChange.Released, null);
        }

        private void Handle(ProgressStarted e)
        {
            var progress = _records.GetProgress(e.AggregateIdentifier, x => x.Gradebook, x => x.GradeItem, x => x.Learner)
                ?? throw new ProgressNotFoundException($"Progress {e.AggregateIdentifier} is not found");

            var course = _courses.FindCourseByGradebook(progress.Gradebook.GradebookIdentifier);
            if (course != null && progress != null)
                ViewUserProgramTaskEnrollment(progress, course);
        }

        private void Handle(ProgressDeleted e)
        {
            var progress = _records.BindProgress(
                x => new { x.UserIdentifier, x.GradeItem.AchievementIdentifier },
                x => x.ProgressIdentifier == e.AggregateIdentifier);
            var credential = progress?.AchievementIdentifier != null
                ? _achievements.GetCredential(progress.AchievementIdentifier.Value, progress.UserIdentifier)
                : null;

            if (credential != null)
                Send(e, new DeleteCredential(credential.CredentialIdentifier));
        }

        private void HandleItemAchievement(IChange change, TriggerCauseChange cause, decimal? percent)
        {
            var progress = _records.GetProgress(change.AggregateIdentifier, x => x.Gradebook, x => x.GradeItem, x => x.Learner)
                ?? throw new ProgressNotFoundException($"Progress {change.AggregateIdentifier} is not found");

            var gradebook = progress.Gradebook;
            var item = progress.GradeItem;

            if (change is ProgressCompleted2 && item.ProgressCompletedMessageIdentifier.HasValue)
            {
                _mailer.Send(gradebook.OrganizationIdentifier, progress.UserIdentifier, new AlertProgressCompleted()
                {
                    MessageIdentifier = item.ProgressCompletedMessageIdentifier.Value,
                    LearnerName = progress.Learner?.UserFullName,
                    LearnerEmail = progress.Learner?.UserEmail,
                    GradeItemName = item.GradeItemName,
                    GradeItemScore = percent.HasValue ? $"{percent:p0}" : string.Empty
                });
            }

            if (item?.AchievementIdentifier == null)
                return;

            // Use the GradebookItemAchievement to determine the side-effects for this process. 

            var whenCause = item.AchievementWhenChange.ToEnumNullable<TriggerCauseChange>();
            if (!whenCause.HasValue || whenCause != cause)
                return;

            var whenGrade = item.AchievementWhenGrade.ToEnumNullable<TriggerCauseGrade>();
            var thenCommand = item.AchievementThenCommand.ToEnumNullable<TriggerEffectCommand>();
            var elseCommand = item.AchievementElseCommand.ToEnumNullable<TriggerEffectCommand>();

            if (!whenGrade.HasValue || !thenCommand.HasValue || !elseCommand.HasValue)
                return;

            // If the grade item is a simple "complete or incomplete" then process it now.

            if (change is ProgressCompleted2 c
                 && StringHelper.Equals(item.GradeItemType, GradeItemType.Score.ToString())
                 && StringHelper.Equals(item.GradeItemFormat, GradeItemFormat.Boolean.ToString())
                 )
            {
                var condition = whenGrade.Value == TriggerCauseGrade.Pass && c.Completed.HasValue
                             || whenGrade.Value == TriggerCauseGrade.Fail && !c.Completed.HasValue;

                var grants = condition
                    ? BuildCommands(thenCommand.Value, gradebook.OrganizationIdentifier, item.AchievementIdentifier.Value, progress.UserIdentifier, c.Completed, null, null)
                    : BuildCommands(elseCommand.Value, gradebook.OrganizationIdentifier, item.AchievementIdentifier.Value, progress.UserIdentifier, c.Completed, null, null);

                foreach (var grant in grants)
                    Send(change, grant);

                TaskProgramCompletion(progress, gradebook, item);

                return;
            }

            // Continue only if there is an item with an achievement condition and a pass percent.
            // Also ignore all record items which are scores but format is not percent.

            if (item.GradeItemPassPercent == null
                || string.Equals(item.GradeItemType, GradeItemType.Score.ToString(), StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(item.GradeItemFormat, GradeItemFormat.Percent.ToString(), StringComparison.OrdinalIgnoreCase)
                )
            {
                return;
            }

            {
                // When the score for a gradebook item is released, the percent value already exists in the Score.

                if (percent == null)
                    percent = progress.ProgressPercent;

                var condition = percent.HasValue
                    && (
                        whenGrade.Value == TriggerCauseGrade.Pass && percent >= item.GradeItemPassPercent
                     || whenGrade.Value == TriggerCauseGrade.Fail && percent < item.GradeItemPassPercent
                    );

                var grade = !percent.HasValue
                    ? "Complete"
                    : percent >= item.GradeItemPassPercent ? "Pass" : "Fail";

                // Only and only one of the two conditions is satisfied at this point. This determines then commands we send
                // for Then and Else cases.

                var reason = $"Score {cause}: {grade} with {percent:p2}";
                var commands = condition
                    ? BuildCommands(thenCommand.Value, gradebook.OrganizationIdentifier, item.AchievementIdentifier.Value, progress.UserIdentifier, item.AchievementFixedDate, reason, percent)
                    : BuildCommands(elseCommand.Value, gradebook.OrganizationIdentifier, item.AchievementIdentifier.Value, progress.UserIdentifier, item.AchievementFixedDate, reason, percent);

                foreach (var command in commands)
                    Send(change, command);

                TaskProgramCompletion(progress, gradebook, item);
            }
        }

        private void TaskProgramCompletion(QProgress progress, QGradebook gradebook, QGradeItem item)
        {
            if (item?.AchievementIdentifier != null && gradebook?.AchievementIdentifier != null && item.AchievementIdentifier.Equals(gradebook.AchievementIdentifier))
            {
                var course = _courses.FindCourseByGradebook(progress.Gradebook.GradebookIdentifier);
                if (course != null)
                {
                    CompleteUserProgramTaskEnrollment(progress, course);
                    SendProgramCompletionNotificationMessage(progress, course);
                }
            }
        }

        private void ViewUserProgramTaskEnrollment(QProgress progress, QCourse course)
        {
            _programStore.TaskViewed(progress.UserIdentifier, course.OrganizationIdentifier, course.CourseIdentifier);
        }

        private void CompleteUserProgramTaskEnrollment(QProgress progress, QCourse course)
        {
            var enrollmentsToCheck = _programStore.TaskCompleted(progress.UserIdentifier, course.OrganizationIdentifier, course.CourseIdentifier);
            foreach (var enrollment in enrollmentsToCheck)
                _programService.CompletionOfProgramAchievement(enrollment.ProgramIdentifier, enrollment.LearnerIdentifier, enrollment.OrganizationIdentifier);
        }

        private void SendProgramCompletionNotificationMessage(QProgress progress, QCourse course)
        {
            var commands = ProgramEnrollment.ProgramEnrollmentCompletion(course.CourseIdentifier, progress.UserIdentifier, course.OrganizationIdentifier
                , _mailer, _messages, _contents, _contacts, _program, _programStore, _achievements);

            if (commands != null)
                foreach (var command in commands.ToArray())
                    _commander.Send(command);
        }

        private ICommand[] BuildCommands(TriggerEffectCommand effect, Guid organization, Guid achievement, Guid user, DateTimeOffset? date, string reason, decimal? percent)
        {
            var commands = new List<ICommand>();
            var credential = _achievements.GetCredential(achievement, user);

            switch (effect)
            {
                case TriggerEffectCommand.Grant:
                    {
                        var id = _achievements.GetCredentialIdentifier(credential?.CredentialIdentifier, achievement, user);
                        var effective = date ?? DateTimeOffset.UtcNow;
                        var person = _contacts.GetPerson(user, organization);
                        commands.Add(new CreateAndGrantCredential(
                            id,
                            organization,
                            achievement,
                            user,
                            effective,
                            "Assigned and granted by gradebook processor.",
                            percent,
                            person?.EmployerGroupIdentifier,
                            person?.EmployerGroupStatus));
                        break;
                    }
                case TriggerEffectCommand.Revoke:
                    {
                        var id = credential != null ? credential.CredentialIdentifier : UuidFactory.Create();
                        var effective = date ?? DateTimeOffset.UtcNow;
                        if (credential == null)
                            commands.Add(new CreateCredential(id, organization, achievement, user, effective));
                        commands.Add(new RevokeCredential(id, DateTimeOffset.UtcNow, reason, percent));
                        break;
                    }
                case TriggerEffectCommand.Void:
                    if (credential != null)
                        commands.Add(new DeleteCredential(credential.CredentialIdentifier));
                    break;
            }

            return commands.ToArray();
        }

        private void HandleCompletedCourse(ProgressCompleted2 e)
        {
            if (e.Pass == false || !e.Pass.HasValue && e.Percent.HasValue && e.Percent != 1)
                return;

            var progress = _aggregateSearch.GetState<ProgressAggregate, ProgressState>(e.AggregateIdentifier);

            var course = _courses.FindCourseByGradebook(progress.Record);
            if (course == null || course.CompletionActivityIdentifier == null)
                return;

            var activity = _courses.FindActivityByGradeItem(progress.Item);
            if (activity == null || activity.ActivityIdentifier != course.CompletionActivityIdentifier)
                return;

            var courseEnrollment = _courses.GetCourseEnrollment(course.CourseIdentifier, progress.User);
            if (courseEnrollment == null || courseEnrollment.CourseCompleted.HasValue)
                return;

            _courseStore.CompleteCourse(course.CourseIdentifier, progress.User, e.ChangeTime);

            if (course.CompletedToLearnerMessageIdentifier == null && course.CompletedToAdministratorMessageIdentifier == null)
                return;

            var user = _contacts.GetUser(courseEnrollment.LearnerUserIdentifier);
            var content = _contents.GetBlock(course.CourseIdentifier);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(user.UserTimeZone);
            var courseStarted = TimeZones.FormatDateOnly(courseEnrollment.CourseStarted, timeZone);
            var organizationCode = _courses.GetOrganizationCode(course.OrganizationIdentifier);

            var notification = new CourseCompletedNotification
            {
                OriginOrganization = e.OriginOrganization,
                OriginUser = e.OriginUser,

                AppUrl = _urls.GetApplicationUrl(organizationCode),

                CourseName = content.Title.GetText(),
                CourseStarted = courseStarted,

                LearnerIdentifier = user.UserIdentifier,
                LearnerFirstName = user.UserFirstName,
                LearnerLastName = user.UserLastName
            };

            if (course.CompletedToAdministratorMessageIdentifier.HasValue)
            {
                notification.MessageIdentifier = course.CompletedToAdministratorMessageIdentifier;
                _mailer.Send(notification, null);
            }

            if (course.CompletedToLearnerMessageIdentifier.HasValue)
            {
                notification.MessageIdentifier = course.CompletedToLearnerMessageIdentifier;
                _mailer.Send(notification, notification.LearnerIdentifier);
                _courseStore.IncreaseMessageCompletedSentCount(courseEnrollment.CourseIdentifier, courseEnrollment.CourseEnrollmentIdentifier);
            }
        }

        private void Send(IChange cause, ICommand effect)
        {
            effect.OriginOrganization = cause.OriginOrganization;
            effect.OriginUser = cause.OriginUser;

            _commander.Send(effect);
        }
    }
}