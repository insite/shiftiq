using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Cases.Write;
using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Issues.Read;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Organizations.Read;
using InSite.Application.People.Write;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Application.Records.Write;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Domain.Surveys.Forms;
using InSite.Domain.Surveys.Sessions;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Surveys.Read
{
    public class ResponseChangeProcessor
    {
        private readonly Urls _urls;
        private readonly ICommander _commander;
        private readonly IAlertMailer _mailer;
        private readonly ICourseObjectSearch _courses;
        private readonly ICaseSearch _issues;
        private readonly IRecordSearch _records;
        private readonly IMessageSearch _messages;
        private readonly ISurveySearch _surveys;
        private readonly IGroupSearch _groups;
        private readonly IContentSearch _contents;
        private readonly IContactSearch _contacts;
        private readonly IProgramSearch _program;
        private readonly IProgramStore _programStore;
        private readonly IProgramService _programService;
        private readonly IAchievementSearch _achievements;
        private readonly IPersonSearch _persons;
        private readonly IOrganizationSearch _organizations;

        public ResponseChangeProcessor(
            Urls urls,
            ICommander commander,
            IChangeQueue publisher,
            IAlertMailer mailer,
            IContactSearch contacts,
            IContentSearch contents,
            ICourseObjectSearch courses,
            IGroupSearch groups,
            ICaseSearch issues,
            IMessageSearch messages,
            IRecordSearch records,
            ISurveySearch surveys,
            IProgramSearch program,
            IProgramStore programStore,
            IProgramService programService,
            IAchievementSearch achievements,
            IPersonSearch persons,
            IOrganizationSearch organizations
            )
        {
            _urls = urls;
            _commander = commander;
            _mailer = mailer;
            _courses = courses;
            _contacts = contacts;
            _contents = contents;
            _groups = groups;
            _issues = issues;
            _messages = messages;
            _records = records;
            _surveys = surveys;
            _program = program;
            _programStore = programStore;
            _programService = programService;
            _achievements = achievements;
            _persons = persons;
            _organizations = organizations;

            publisher.Subscribe<ResponseSessionStarted>(Handle);
            publisher.Subscribe<ResponseSessionCompleted>(Handle);
            publisher.Subscribe<ResponseSessionDeleted>(Handle);
        }

        public void Handle(ResponseSessionStarted e)
        {
            var session = _surveys.GetResponseSession(e.AggregateIdentifier, x => x.Respondent, x => x.SurveyForm);
            if (session == null)
                return;

            if (!string.IsNullOrEmpty(session.SurveyForm.SurveyFormHook))
                UpdateGradebook(session.RespondentUserIdentifier, session.SurveyForm.SurveyFormHook, false, false);

            if (session.SurveyForm.SurveyMessageResponseStarted.HasValue)
            {
                var notification = CreateNotification(e, session.SurveyForm.SurveyMessageResponseStarted.Value, session);
                SendNotification(notification, null);
            }

            ViewUserProgramTaskEnrollment(session);
        }

        public void Handle(ResponseSessionCompleted e)
        {
            var session = _surveys.GetResponseSession(
                e.AggregateIdentifier,
                x => x.Respondent, x => x.SurveyForm,
                x => x.QResponseAnswers, x => x.QResponseOptions);

            if (session == null)
                return;

            SendMessageToAdministrator(session, e);
            SendMessageToRespondent(session, e);

            CompleteUserProgramTaskEnrollment(session);
            SendProgramCompletionNotificationMessage(session, e);
            TryAddUserToReviewQueue(session, e);
            TryOpenIssue(e, session);
        }

        private void TryOpenIssue(ResponseSessionCompleted e, QResponseSession session)
        {
            var survey = _surveys.GetSurveyState(session.SurveyFormIdentifier);
            if (survey?.WorkflowConfiguration == null)
                return;

            var questions = survey.Form.Questions.Where(x => x.EnableCreateCase).ToArray();
            if (questions.Length == 0)
                return;

            var hasAnswer = false;
            foreach (var question in questions)
            {
                var hasSelectedOption = session.QResponseOptions.Any(
                    x => x.SurveyQuestionIdentifier == question.Identifier && x.ResponseOptionIsSelected);
                if (hasSelectedOption)
                {
                    hasAnswer = true;
                    break;
                }

                var hasTextAnswer = session.QResponseAnswers.Any(
                    x => x.SurveyQuestionIdentifier == question.Identifier && x.ResponseAnswerText.IsNotEmpty());
                if (hasTextAnswer)
                {
                    hasAnswer = true;
                    break;
                }
            }

            if (hasAnswer)
                OpenIssue(survey.WorkflowConfiguration, session, e);
        }

        private void ViewUserProgramTaskEnrollment(QResponseSession session)
        {
            _programStore.TaskViewed(session.RespondentUserIdentifier, session.OrganizationIdentifier, session.SurveyFormIdentifier);
        }

        private void CompleteUserProgramTaskEnrollment(QResponseSession session)
        {
            var enrollmentsToCheck = _programStore.TaskCompleted(session.RespondentUserIdentifier, session.OrganizationIdentifier, session.SurveyFormIdentifier);
            foreach (var enrollment in enrollmentsToCheck)
                _programService.CompletionOfProgramAchievement(enrollment.ProgramIdentifier, enrollment.LearnerIdentifier, enrollment.OrganizationIdentifier);
        }

        private void SendProgramCompletionNotificationMessage(QResponseSession session, ResponseSessionCompleted e)
        {
            var commands = ProgramEnrollment.ProgramEnrollmentCompletion(session.SurveyFormIdentifier, session.RespondentUserIdentifier, session.OrganizationIdentifier
                , _mailer, _messages, _contents, _contacts, _program, _programStore, _achievements);

            if (commands != null)
                foreach (var command in commands.ToArray())
                    _commander.Send(command);
        }

        private void SendMessageToAdministrator(QResponseSession session, ResponseSessionCompleted e)
        {
            if (!string.IsNullOrEmpty(session.SurveyForm.SurveyFormHook))
                UpdateGradebook(session.RespondentUserIdentifier, session.SurveyForm.SurveyFormHook, true, false);

            var activity = _courses.FindActivityBySurveyForm(session.SurveyFormIdentifier);
            if (activity != null)
            {
                var gradebook = activity.Module?.Unit?.Course?.GradebookIdentifier;
                var gradeitem = activity.GradeItem?.GradeItemIdentifier;
                if (gradebook.HasValue && gradeitem.HasValue)
                {
                    var progress = GetProgress(gradebook.Value, session.RespondentUserIdentifier, gradeitem.Value);
                    if (progress != null && !progress.ProgressIsCompleted)
                        _commander.Send(new CompleteProgress(progress.ProgressIdentifier, e.ChangeTime, 1, null, null));
                }
            }

            if (session.SurveyForm.SurveyMessageResponseCompleted.HasValue)
            {
                var notification = CreateNotification(e, session.SurveyForm.SurveyMessageResponseCompleted.Value, session);
                SendNotification(notification, null);
            }
        }

        public void SendMessageToRespondent(QResponseSession session, ResponseSessionCompleted e)
        {
            if (session.SurveyForm.SurveyMessageResponseConfirmed.HasValue)
            {
                var notification = CreateNotification(e, session.SurveyForm.SurveyMessageResponseConfirmed.Value, session);
                SendNotification(notification, session.RespondentUserIdentifier);
            }
        }

        private void TryAddUserToReviewQueue(QResponseSession session, ResponseSessionCompleted e)
        {
            var isGroupExists = _groups.GroupExists(new QGroupFilter
            {
                SurveyFormIdentifier = session.SurveyFormIdentifier,
                AllowSelfSubscription = true,
                SurveyNecessity = Necessity.Required.GetName()
            });

            if (!isGroupExists)
                return;

            var person = _persons.GetPerson(session.RespondentUserIdentifier, session.OrganizationIdentifier);
            if (person == null)
                return;

            if (person.AccountReviewQueued.HasValue || person.AccountReviewCompleted.HasValue)
                return;

            _commander.Send(new ModifyPersonFieldDateOffset(person.PersonIdentifier, PersonField.AccountReviewQueued, DateTimeOffset.UtcNow));
        }

        private void OpenIssue(SurveyWorkflowConfiguration workflow, QResponseSession session, ResponseSessionCompleted e)
        {
            if (workflow == null)
                return;

            var issues = _issues.GetIssues(new QIssueFilter
            {
                OrganizationIdentifier = e.OriginOrganization,
                ResponseSessionIdentifier = session.ResponseSessionIdentifier
            });

            if (issues != null && issues.Count > 0)
                return;

            var survey = session.SurveyForm;

            var id = UuidFactory.Create();
            var type = workflow.IssueType;
            var number = _issues.GetNextIssueNumber(e.OriginOrganization);
            var person = _contacts.GetPerson(session.Respondent.UserIdentifier, e.OriginOrganization);
            var personCode = person?.PersonCode;
            var personEmployer = person?.EmployerGroupIdentifier;
            var title = $"{survey.SurveyFormName} - {session.Respondent.UserFullName} {personCode}";
            var now = e.ChangeTime;

            _commander.Send(new OpenIssue(id, e.OriginOrganization, number, title, null, now, "Survey Response", type, null)
            {
                OriginOrganization = e.OriginOrganization
            });

            _commander.Send(new ConnectIssueToSurveyResponse(id, session.ResponseSessionIdentifier)
            {
                OriginOrganization = e.OriginOrganization
            });

            var administrator = workflow.AdministratorUserIdentifier;
            if (administrator.HasValue)
            {
                _commander.Send(new AssignUser(id, administrator.Value, "Administrator")
                {
                    OriginOrganization = e.OriginOrganization
                });
            }

            var owner = workflow.OwnerUserIdentifier;
            if (owner.HasValue)
            {
                _commander.Send(new AssignUser(id, owner.Value, "Owner")
                {
                    OriginOrganization = e.OriginOrganization
                });
            }

            _commander.Send(new AssignUser(id, session.RespondentUserIdentifier, "Topic")
            {
                OriginOrganization = e.OriginOrganization
            });

            var status = workflow.IssueStatusIdentifier;
            if (status.HasValue)
            {
                _commander.Send(new ChangeIssueStatus(id, status.Value, now)
                {
                    OriginOrganization = e.OriginOrganization
                });
            }

            if (personEmployer.HasValue)
            {
                _commander.Send(new AssignGroup(id, personEmployer.Value, "Employer")
                {
                    OriginOrganization = e.OriginOrganization
                });
            }
        }

        public void Handle(ResponseSessionDeleted e)
        {
            var session = _surveys.GetResponseSession(e.AggregateIdentifier, x => x.SurveyForm);
            if (session != null)
            {
                if (!string.IsNullOrEmpty(session.SurveyForm.SurveyFormHook))
                    UpdateGradebook(session.RespondentUserIdentifier, session.SurveyForm.SurveyFormHook, false, true, false);
            }
        }

        private void UpdateGradebook(Guid user, string hook, bool complete, bool delete, bool calculate = true)
        {
            var item = GetItem(hook);
            if (item == null)
                return;
            UpdateGradebook(user, item.GradebookIdentifier, item.GradeItemIdentifier, complete, delete, calculate);
        }

        private void UpdateGradebook(Guid user, Guid gradebook, Guid gradeitem, bool complete, bool delete, bool calculate = true)
        {
            var progressId = _records.GetProgressIdentifier(gradebook, gradeitem, user);

            if (delete)
            {
                if (progressId.HasValue)
                    _commander.Send(new DeleteProgress(progressId.Value));
            }
            else
            {
                CourseObjectChangeProcessor.EnsureEnrollment(_commander.Send, _records, gradebook, user, DateTimeOffset.UtcNow);

                var progressIdentifier = progressId
                    ?? CreateProgress(gradebook, user, gradeitem);

                if (complete)
                    _commander.Send(new CompleteProgress(progressIdentifier, DateTimeOffset.UtcNow, 1, null, null));
                else
                    _commander.Send(new StartProgress(progressIdentifier, DateTimeOffset.UtcNow));
            }

            if (complete && calculate)
            {
                var calculateCommands = GradebookCalculator.Calculate(gradebook, user, false, _records);
                foreach (var command in calculateCommands)
                    _commander.Send(command);
            }
        }

        private QProgress GetProgress(Guid record, Guid user, Guid item)
        {
            var progress = _records.GetProgress(record, item, user);
            if (progress == null)
            {
                CreateProgress(record, user, item);
                progress = _records.GetProgress(record, item, user);
            }
            return progress;
        }

        private Guid CreateProgress(Guid gradebook, Guid user, Guid item)
        {
            CourseObjectChangeProcessor.EnsureEnrollment(_commander.Send, _records, gradebook, user, DateTimeOffset.UtcNow);

            var command = _records.CreateCommandToAddProgress(null, gradebook, item, user);

            _commander.Send(command);

            return command.AggregateIdentifier;
        }

        private QGradeItem GetItem(string hook)
        {
            return _records.GetGradeItemByHook(hook);
        }

        private ResponseNotification CreateNotification(Change change, Guid? messageId, QResponseSession session)
        {
            ResponseNotification notification = null;

            if (change is ResponseSessionStarted)
            {
                notification = new ResponseStartedNotification();
            }
            else if (change is ResponseSessionCompleted)
            {
                notification = new ResponseCompletedNotification();
            }
            else if (change is ResponseSessionConfirmed)
            {
                notification = new ResponseConfirmedNotification();
            }

            notification.MessageIdentifier = messageId;

            notification.OriginOrganization = change.OriginOrganization;
            notification.OriginUser = change.OriginUser;

            var organization = _organizations.Get(session.OrganizationIdentifier);

            notification.AppUrl = _urls.GetApplicationUrl(organization.OrganizationCode);

            notification.CurrentYear = DateTime.Now.Year.ToString();
            notification.OrganizationName = organization.CompanyName;
            notification.SurveyFormName = session.SurveyForm.SurveyFormName;
            notification.Tenant = organization.CompanyName;
            notification.UserEmail = session.Respondent.UserEmail;
            notification.UserFullName = session.Respondent.UserFullName;
            notification.UserIdentifier = session.Respondent.UserIdentifier.ToString();
            notification.UtcNow = string.Format("{0:MMM d, yyyy} {0:HH:mm:ss tt} UTC", DateTime.UtcNow);

            return notification;
        }

        private void SendNotification(Notification notification, Guid? to)
        {
            if (notification == null)
                return;

            _mailer.Send(notification, to);
        }
    }
}