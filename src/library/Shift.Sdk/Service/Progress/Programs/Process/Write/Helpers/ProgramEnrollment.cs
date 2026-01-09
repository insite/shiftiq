using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.Credentials.Write;
using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Records.Write
{
    public class ProgramEnrollment
    {
        private readonly IAlertMailer _mailer;
        private readonly IMessageSearch _messages;
        private readonly IContentSearch _contents;
        private readonly IContactSearch _contacts;
        private readonly IProgramSearch _programs;
        private readonly IProgramStore _programsStore;
        private readonly IAchievementSearch _achievements;

        private List<Command> _commands;

        public ProgramEnrollment(IAlertMailer mailer, IMessageSearch messages, IContentSearch contents, IContactSearch contacts, IProgramSearch programs, IProgramStore programsStore, IAchievementSearch achievements)
        {
            _mailer = mailer;
            _messages = messages;
            _contents = contents;
            _contacts = contacts;
            _programs = programs;
            _achievements = achievements;
            _programsStore = programsStore;
            _commands = new List<Command>();
        }

        public static List<Command> ProgramEnrollmentCompletion(Guid objectId, Guid userId, Guid organization,
            IAlertMailer mailer, IMessageSearch messages, IContentSearch contents, IContactSearch contacts, IProgramSearch _programs, IProgramStore _programsStore, IAchievementSearch _achievements)
        {
            var programs = _programs.GetProgramIds(objectId);

            return new ProgramEnrollment(mailer, messages, contents, contacts, _programs, _programsStore, _achievements).ProgramEnrollmentCompletion(objectId, userId, organization, programs);
        }

        public static List<Command> ProgramEnrollmentForStandAloneAchievementCompletion(Guid objectId, Guid userId, Guid organization,
            IAlertMailer mailer, IMessageSearch messages, IContentSearch contents, IContactSearch contacts, IProgramSearch _programs, IProgramStore _programsStore, IAchievementSearch _achievements)
        {
            var programs = _programs.GetProgramIdsForStandaloneAchievements(objectId, out var objects);

            List<Command> commands = new List<Command>();
            foreach (var objectItem in objects)
                commands.AddRange(new ProgramEnrollment(mailer, messages, contents, contacts, _programs, _programsStore, _achievements).ProgramEnrollmentCompletion(objectItem.Value, userId, organization, programs));

            return commands;
        }

        private List<Command> ProgramEnrollmentCompletion(Guid objectId, Guid userId, Guid organizationId, List<Guid> programs)
        {
            if (programs != null && programs.Count > 0)
            {
                foreach (var program in programs)
                {
                    var values = _programs.GetProgramValues(program, objectId);
                    if (values == null)
                        continue;

                    if ((values.CompletionTaskIdentifier.HasValue && values.TaskIdentifier.HasValue && values.TaskIdentifier.Value.Equals(values.CompletionTaskIdentifier.Value)) ||
                        _programs.IsProgramFullyCompletedByLearner(values.ProgramIdentifier, userId))
                    {
                        var user = _contacts.GetUser(userId);
                        var person = _contacts.GetPerson(userId, organizationId);
                        if (person != null)
                        {
                            var email = EmailAddress.GetEnabledEmail(user.UserEmail, person.UserEmailEnabled, user.UserEmailAlternate, person.UserEmailAlternateEnabled);
                            if (email.IsEmpty())
                                continue;

                            if (values.AchievementIdentifier.HasValue)
                                BuildCommands(TriggerEffectCommand.Grant, organizationId, values.AchievementIdentifier.Value, user.UserIdentifier, _commands, _achievements, _contacts);

                            if (values.NotificationCompletedLearnerMessageIdentifier.HasValue)
                            {
                                var notification = CompletedNotification(organizationId, values.NotificationCompletedLearnerMessageIdentifier.Value, user.UserFirstName, user.UserLastName, values.ProgramName, null);
                                SendNotification(notification, user.UserIdentifier);
                            }

                            if (values.NotificationCompletedAdministratorMessageIdentifier.HasValue)
                            {
                                var notification = CompletedNotification(organizationId, values.NotificationCompletedAdministratorMessageIdentifier.Value, user.UserFirstName, user.UserLastName, values.ProgramName, null);
                                SendNotification(notification, null);
                            }

                            _programsStore.ProgramCompleted(values.ProgramIdentifier, userId, organizationId, values.AchievementIdentifier);
                        }
                    }
                }
            }

            return _commands;
        }

        public static void BuildCommands(
            TriggerEffectCommand effect,
            Guid organization,
            Guid achievement,
            Guid user,
            List<Command> commands,
            IAchievementSearch achievementSearch,
            IContactSearch contactSearch
            )
        {
            if (effect != TriggerEffectCommand.Grant)
                return;

            var credential = achievementSearch.GetCredential(achievement, user);
            var id = achievementSearch.GetCredentialIdentifier(credential?.CredentialIdentifier, achievement, user);
            var person = contactSearch.GetPerson(user, organization);

            var command = new CreateAndGrantCredential(
                id,
                organization,
                achievement,
                user,
                DateTimeOffset.UtcNow,
                "Assigned and granted on completion of program.",
                null,
                person?.EmployerGroupIdentifier,
                person?.EmployerGroupStatus
            );

            commands.Add(command);
        }

        public ProgramCompletedNotification CompletedNotification(Guid organizationId, Guid messageId, string learnerFirstName, string learnerLastName, string programName, string programStarted)
        {
            var notification = new ProgramCompletedNotification
            {
                OriginOrganization = organizationId,
                LearnerFirstName = learnerFirstName,
                LearnerLastName = learnerLastName,
                MessageIdentifier = messageId,
                ProgramName = programName,
                ProgramStarted = programStarted
            };

            return notification;
        }

        public ProgramStalledNotification StalledNotification(Guid organizationId, Guid messageId, string learnerFirstName, string learnerLastName, string programName, string programStarted)
        {
            var notification = new ProgramStalledNotification
            {
                OriginOrganization = organizationId,
                LearnerFirstName = learnerFirstName,
                LearnerLastName = learnerLastName,
                MessageIdentifier = messageId,
                ProgramName = programName,
                ProgramStarted = programStarted
            };

            return notification;
        }

        public void SendNotification(Notification notification, Guid? to)
        {
            if (notification == null)
                return;

            _mailer.Send(notification, to);
        }
    }
}
