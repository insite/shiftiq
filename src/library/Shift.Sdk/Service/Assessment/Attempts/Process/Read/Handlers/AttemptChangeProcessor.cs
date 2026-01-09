using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Messages.Write;
using InSite.Application.Organizations.Read;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Application.Registrations.Write;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Attempts.Read
{
    /// <summary>
    /// Implements the process manager for Attempt events. 
    /// </summary>
    /// <remarks>
    /// Event handlers in this class have side effects.
    /// </remarks>
    public class AttemptChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IAlertMailer _mailer;

        private readonly IAttemptSearch _attemptSearch;

        private readonly IBankSearch _banks;
        private readonly ICourseObjectSearch _courses;
        private readonly IContactSearch _contacts;
        private readonly IRecordSearch _records;
        private readonly IOrganizationSearch _organizations;

        public AttemptChangeProcessor(ICommander commander, IChangeQueue publisher, IAlertMailer mailer,
            IAttemptSearch attemptSearch, IBankSearch banks, IContactSearch contacts, ICourseObjectSearch courses,
            IRecordSearch records, IOrganizationSearch organizations)
        {
            _commander = commander;
            _mailer = mailer;

            _attemptSearch = attemptSearch;

            _banks = banks;
            _contacts = contacts;
            _courses = courses;
            _records = records;
            _organizations = organizations;

            publisher.Subscribe<AttemptImported>(Handle);
            publisher.Subscribe<AttemptStarted3>(Handle);
            publisher.Subscribe<AttemptSubmitted>(Handle);
            publisher.Subscribe<AttemptGraded>(Handle);
            publisher.Subscribe<AttemptAnalyzed>(Handle);
        }

        public void Handle(AttemptImported e)
        {
            // Analyze the attempt immediately after importing it.
            _commander.Send(new AnalyzeAttempt(e.AggregateIdentifier, e.IsAttended));

            // Link the attempt to the registration.
            if (e.Registration.HasValue)
                _commander.Send(new AssignAttempt(e.Registration.Value, e.AggregateIdentifier));
        }

        public void Handle(AttemptStarted3 e)
        {
            var attempt = _attemptSearch.GetAttempt(e.AggregateIdentifier);
            var form = _banks.GetForm(attempt.FormIdentifier);
            var learner = _contacts.GetUser(attempt.LearnerUserIdentifier);

            string formName = form.FormName;
            string userEmail = learner.UserEmail;
            string userName = learner.UserFullName;

            // Link the attempt to the registration.
            if (e.RegistrationIdentifier.HasValue)
                _commander.Send(new AssignAttempt(e.RegistrationIdentifier.Value, e.AggregateIdentifier));

            try
            {
                _mailer.Send(new AssessmentAttemptStartedNotification
                {
                    OriginOrganization = attempt.OrganizationIdentifier,
                    OriginUser = e.OriginUser,

                    LearnerEmail = userEmail,
                    LearnerName = userName,
                    AssessmentFormName = formName,
                });
            }
            catch { }
        }

        public void Handle(AttemptSubmitted e)
        {
            var command = e.Grade
                ? (ICommand)new GradeAttempt(e.AggregateIdentifier)
                : new AnalyzeAttempt(e.AggregateIdentifier, true);

            command.OriginOrganization = e.OriginOrganization;
            command.OriginUser = e.OriginUser;

            _commander.Send(command);
        }

        public void Handle(AttemptGraded e)
        {
            var attempt = _attemptSearch.GetAttempt(e.AggregateIdentifier);
            if (attempt == null)
                return;

            var form = _banks.GetForm(attempt.FormIdentifier);
            var score = attempt.AttemptScore ?? 0;
            var gradeItems = FindGradeItems(form);

            if (gradeItems.Count > 0)
                UpdateGradebook(
                    attempt.LearnerUserIdentifier,
                    gradeItems,
                    score,
                    attempt.AttemptIsPassing,
                    (int?)attempt.AttemptDuration);

            _commander.Send(
                new AnalyzeAttempt(e.AggregateIdentifier, true)
                {
                    OriginOrganization = e.OriginOrganization,
                    OriginUser = e.OriginUser
                });
            _commander.Send(
                new AnalyzeForm(form.BankIdentifier, form.FormIdentifier)
                {
                    OriginOrganization = e.OriginOrganization,
                    OriginUser = e.OriginUser
                });

            SendEmailNotification(attempt, score, e.OriginUser, form);
        }

        public void Handle(AttemptAnalyzed e)
        {
            var attempt = _attemptSearch.GetAttempt(e.AggregateIdentifier, x => x.Registration.Event.VenueLocation);
            if (attempt == null)
                return;

            FinalizeRegistration(attempt, e.AllowTakeAttendance);
            PostLearnerCommentsToAssessmentBank(attempt);
            PostLearnerScoresToGradebook(attempt);
        }

        private void FinalizeRegistration(QAttempt attempt, bool takeAttendance)
        {
            var registration = attempt.Registration;
            if (registration == null)
                return;

            var @event = registration.Event;
            var hasEvent = @event != null;

            // Indicate the final score and grade on the learner's registration.
            var score = attempt.AttemptScore ?? 0;
            var grade = attempt.AttemptIsPassing ? "Pass" : "Fail";
            _commander.Send(new ChangeGrade(registration.RegistrationIdentifier, grade, score));

            // If attendance is requested then mark the learner as present for the event.
            if (takeAttendance)
            {
                var command = new TakeAttendance(registration.RegistrationIdentifier, "Present", null, null);

                if (hasEvent)
                {
                    command.Quantity = @event.DurationQuantity;
                    command.Unit = @event.DurationUnit;
                }

                _commander.Send(command);
            }

            // If the attempt was scheduled then ensure it is tagged.
            if (hasEvent && attempt.AttemptTag == null)
                _commander.Send(new TagAttempt(attempt.AttemptIdentifier, @event.VenueLocationName));
        }

        private void PostLearnerCommentsToAssessmentBank(QAttempt attempt)
        {
            if (attempt == null)
                return;

            var comments = _attemptSearch.GetVAttemptComments(attempt.AttemptIdentifier);
            if (comments.Count == 0)
                return;

            var form = _banks.GetForm(attempt.FormIdentifier);
            if (form == null)
                return;

            Guid? venueLocationId = null;
            DateTimeOffset? eventScheduledStart = null;
            {
                var @event = attempt.Registration?.Event;
                if (@event != null)
                {
                    venueLocationId = @event.VenueLocationIdentifier;
                    eventScheduledStart = @event.EventScheduledStart;
                }
            }

            foreach (var comment in comments)
            {
                var existing = _banks.GetComment(comment.CommentIdentifier);
                if (existing == null || (existing.ContainerType == "Assessment Attempt" && existing.ContainerIdentifier == attempt.AttemptIdentifier))
                    _commander.Send(new PostComment(
                          form.BankIdentifier,
                          comment.CommentIdentifier,
                          FlagType.Blue,
                          CommentType.Question,
                          comment.AssessmentQuestionIdentifier ?? Guid.Empty,
                          comment.AuthorUserIdentifier,
                          CommentAuthorType.Candidate,
                          null,
                          comment.CommentText,
                          venueLocationId,
                          eventScheduledStart,
                          "Online",
                          comment.CommentPosted));
                else
                    _commander.Send(new ReviseComment(
                          form.BankIdentifier,
                          comment.CommentIdentifier,
                          comment.AuthorUserIdentifier,
                          FlagType.Blue,
                          existing.CommentCategory,
                          comment.CommentText,
                          venueLocationId,
                          eventScheduledStart,
                          "Online",
                          comment.CommentRevised ?? DateTimeOffset.UtcNow));
            }
        }

        private void SendEmailNotification(QAttempt attempt, decimal score, Guid user, QBankForm form)
        {
            try
            {
                var learner = _contacts.GetUser(attempt.LearnerUserIdentifier);
                _mailer.Send(new AssessmentAttemptCompletedNotification
                {
                    OriginOrganization = attempt.OrganizationIdentifier,
                    OriginUser = user,

                    LearnerEmail = learner.UserEmail,
                    LearnerName = learner.UserFullName,
                    AssessmentFormName = form.FormName,
                    AssessmentAttemptScore = $"{score:p0}"
                });
            }
            catch
            {
                // If email cannot be delivered just ignore it (maybe it is just wrong email address)
            }
        }

        #region Form-based Gradebook Integration

        private void PostLearnerScoresToGradebook(QAttempt attempt)
        {
            var attemptQuestions = _attemptSearch.GetAttemptQuestions(attempt.AttemptIdentifier);
            if (attemptQuestions.IsEmpty())
                return;

            var form = _banks.GetFormData(attempt.FormIdentifier);
            if (form == null || form.Gradebook == null)
                return;

            var gradebookId = form.Gradebook.Value;
            var learnerId = attempt.LearnerUserIdentifier;
            var commands = new List<Command>();

            AddProgressCommands(form, learnerId, attemptQuestions, commands);

            if (commands.Count == 0)
                return;

            var calculateCommands = GradebookCalculator.Calculate(gradebookId, learnerId, false, _records);
            commands.AddRange(calculateCommands);

            var gradebook = _records.GetGradebookState(gradebookId);
            if (gradebook.IsLocked)
            {
                commands.Insert(0, new UnlockGradebook(gradebookId));
                commands.Add(new LockGradebook(gradebookId));
            }

            foreach (var command in commands)
                _commander.Send(command);
        }

        private void AddProgressCommands(
            Form form,
            Guid learnerId,
            List<QAttemptQuestion> attemptQuestions,
            List<Command> commands
            )
        {
            var questionIds = new HashSet<Guid>(attemptQuestions.Select(x => x.QuestionIdentifier));

            var questionGradeItems = GetQuestionGradeItems(form, questionIds);
            if (questionGradeItems.IsEmpty())
                return;

            var gradebookId = form.Gradebook.Value;
            if (!_records.GradebookExists(gradebookId))
                return;

            if (!_records.EnrollmentExists(gradebookId, learnerId))
                commands.Add(new AddEnrollment(gradebookId, UuidFactory.Create(), learnerId, null, DateTimeOffset.Now, null));

            foreach (var (questionId, gradeItemId) in questionGradeItems)
            {
                var attemptQuestion = attemptQuestions.First(x => x.QuestionIdentifier == questionId);
                var progressId = _records.GetProgressIdentifier(gradebookId, gradeItemId, learnerId);

                if (!progressId.HasValue)
                {
                    if (!_records.GradeItemExists(gradeItemId))
                        continue;

                    progressId = UuidFactory.Create();
                    commands.Add(new AddProgress(progressId.Value, gradebookId, gradeItemId, learnerId));
                }

                commands.Add(new ChangeProgressPoints(progressId.Value, attemptQuestion.AnswerPoints, attemptQuestion.QuestionPoints, DateTimeOffset.UtcNow));
            }
        }

        private List<(Guid, Guid)> GetQuestionGradeItems(Form form, HashSet<Guid> questionIds)
        {
            var result = new List<(Guid, Guid)>();

            var questions = form.GetQuestions();
            foreach (var question in questions)
            {
                if (questionIds.Contains(question.Identifier)
                    && question.GradeItems.TryGetValue(form.Identifier, out var gradeItemId)
                    )
                {
                    result.Add((question.Identifier, gradeItemId));
                }

                if (question.Likert == null)
                    continue;

                foreach (var row in question.Likert.Rows)
                {
                    if (questionIds.Contains(row.Identifier)
                        && row.GradeItems.TryGetValue(form.Identifier, out var rowGradeItemId)
                        )
                    {
                        result.Add((row.Identifier, rowGradeItemId));
                    }
                }
            }

            return result;
        }

        #endregion

        #region Course-based Gradebook Integration

        private void UpdateGradebook(Guid user, List<Guid> gradeItems, decimal percent, bool pass, int? elapsedSeconds)
        {
            var unlockedItems = _records.BindGradeItems(
                x => new
                {
                    x.GradebookIdentifier,
                    x.GradeItemIdentifier
                },
                x => !x.Gradebook.IsLocked && gradeItems.Contains(x.GradeItemIdentifier));

            foreach (var gradebook in unlockedItems.GroupBy(x => x.GradebookIdentifier))
            {
                var gradebookId = gradebook.Key;

                CourseObjectChangeProcessor.EnsureEnrollment(_commander.Send, _records, gradebookId, user, DateTimeOffset.UtcNow);

                var progressions = GetProgresses(gradebookId, user, gradebook.Select(x => x.GradeItemIdentifier));
                foreach (var progression in progressions)
                    _commander.Send(new CompleteProgress(progression, DateTimeOffset.UtcNow, percent, pass, null, elapsedSeconds));

                var commands = GradebookCalculator.Calculate(gradebookId, user, false, _records);
                foreach (var command in commands)
                    _commander.Send(command);
            }
        }

        private List<Guid> FindGradeItems(QBankForm form)
        {
            var direct = _courses.FindActivityByAssessmentForm(form.FormIdentifier)?.GradeItemIdentifier;

            var indirect = !string.IsNullOrEmpty(form.FormHook) ? _records.GetGradeItemByHook(form.FormHook)?.GradeItemIdentifier : null;

            var result = new List<Guid>();

            if (direct.HasValue)
                result.Add(direct.Value);

            if (indirect.HasValue)
                result.Add(indirect.Value);

            return result;
        }

        private List<Guid> GetProgresses(Guid gradebookId, Guid userId, IEnumerable<Guid> gradeItemIds)
        {
            var existProgresses = _records
                .BindProgresses(
                    x => new
                    {
                        x.ProgressIdentifier,
                        x.GradeItemIdentifier
                    },
                    x => x.GradebookIdentifier == gradebookId
                      && x.UserIdentifier == userId
                      && gradeItemIds.Contains(x.GradeItemIdentifier)
                );
            var newProgresses = new List<Guid>();

            var commands = _records.CreateCommandsToAddProgresses(
                null,
                gradebookId, userId,
                gradeItemIds.Where(x => existProgresses.All(y => y.GradeItemIdentifier != x)));

            foreach (var command in commands)
            {
                _commander.Send(command);

                newProgresses.Add(command.AggregateIdentifier);
            }

            return existProgresses.Select(x => x.ProgressIdentifier).Concat(newProgresses).ToList();
        }

        #endregion
    }
}