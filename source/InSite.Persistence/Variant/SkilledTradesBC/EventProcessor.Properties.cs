using System;

using Shift.Common.Timeline.Changes;

using InSite.Application;
using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Logs.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Registrations.Read;
using InSite.Application.Standards.Read;
using InSite.Domain.Events;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    public partial class EventProcessor
    {
        private readonly Urls _urls;
        private readonly IChangeRepository _changes;
        private readonly ICommandSearch _commands;

        private readonly CommandBroker _broker;
        private readonly MessageBuilder _builder;

        private readonly IEventSearch _events;
        private readonly IBankSearch _assessments;
        private readonly IAttemptSearch _attempts;
        private readonly IContactSearch _contacts;
        private readonly IRegistrationSearch _registrations;
        private readonly IOldStandardSearch _standards;
        private readonly IGroupSearch _groups;

        private readonly IDirectAccessClient _da;

        private readonly Action<Exception> _error;
        private readonly Action<string, string> _warning;

        private readonly IAlertMailer _mailgun;

        private readonly FilePaths _filePaths;
        private readonly string _domain;

        private readonly EventProcessorHelper _helper;

        public EventProcessor(
            Urls urls,
            ICommander commander, IChangeQueue publisher,
            ICommandSearch commands, IChangeRepository changes,
            IEventSearch events, IBankSearch assessments, IAttemptSearch attempts,
            IContactSearch contacts, IRegistrationSearch registrations, IOldStandardSearch standards,
            IGroupSearch groups,
            IDirectAccessClient da,
            Action<string, string> warning, Action<Exception> error,
            IAlertMailer mailer,
            FilePaths filePaths,
            string domain
            )
        {
            _urls = urls;

            var ita = OrganizationIdentifiers.SkilledTradesBC;

            publisher.Extend<ExamScheduled2>(Handle, ita);
            publisher.Extend<EventFormatChanged>(Handle, ita);
            publisher.Extend<EventRescheduled>(Handle, ita);
            publisher.Extend<EventPublicationStarted>(Handle, ita);
            publisher.Extend<EventPublicationCompleted>(Handle, ita);
            publisher.Extend<EventCancelled>(Handle, ita);

            publisher.Extend<EventScheduleStatusChanged>(Handle, ita);
            publisher.Extend<EventVenueChanged2>(Handle, ita);
            publisher.Extend<EventNotificationTriggered>(Handle, ita);

            publisher.Extend<EventScoresValidated>(Handle, ita);
            publisher.Extend<EventScoresPublished>(Handle, ita);

            publisher.Extend<DistributionTracked>(Handle, ita);
            publisher.Extend<ExamMaterialReturned>(Handle, ita);

            _broker = new CommandBroker(commander);
            _changes = changes;

            _events = events;
            _attempts = attempts;
            _assessments = assessments;
            _commands = commands;
            _contacts = contacts;
            _registrations = registrations;
            _standards = standards;
            _groups = groups;
            _helper = new EventProcessorHelper(_groups);

            _da = da;

            _warning = warning;
            _error = error;
            _mailgun = mailer;

            _filePaths = filePaths;

            _domain = domain;

            _builder = new MessageBuilder(_contacts, groups, filePaths, _domain);
        }
    }
}
