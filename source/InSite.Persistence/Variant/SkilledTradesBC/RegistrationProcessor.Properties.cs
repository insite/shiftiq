using System;

using Shift.Common.Timeline.Changes;

using InSite.Application;
using InSite.Application.Banks.Read;
using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Logs.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Registrations.Read;
using InSite.Domain.Registrations;
using InSite.Persistence.Integration.DirectAccess;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Integration.DirectAccess;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    public partial class RegistrationProcessor
    {
        private readonly ICommandSearch _commands;

        private readonly CommandBroker _broker;

        private readonly IEventSearch _events;
        private readonly IBankSearch _banks;
        private readonly IContactSearch _contacts;
        private readonly IRegistrationSearch _registrations;
        private readonly IRegistrationStore _registrationStore;
        private readonly IGroupSearch _groups;

        private readonly IDirectAccessClient _da;
        private readonly IDirectAccessStore _daStore;

        private readonly Action<string, string> _warning;
        private readonly Action<Exception> _error;
        private readonly IAlertMailer _mailgun;

        private readonly FilePaths _filePaths;

        private readonly string _domain;

        public RegistrationProcessor(ICommander commander, IChangeQueue publisher,
            ICommandSearch commands,
            IEventSearch events, IBankSearch assessments, IContactSearch contacts, IRegistrationSearch registrations, IRegistrationStore registrationStore,
            IGroupSearch groups,
            IDirectAccessClient da, IDirectAccessStore daStore,
            Action<string, string> warning, Action<Exception> error,
            IAlertMailer mailgun,
            FilePaths filePaths,
            string domain
            )
        {
            var ita = OrganizationIdentifiers.SkilledTradesBC;

            publisher.Extend<SynchronizationChanged>(Handle, ita);
            publisher.Extend<ExamFormAssigned>(Handle, ita);
            publisher.Extend<EligibilityChanged>(Handle, ita);
            publisher.Extend<ApprovalChanged>(Handle, ita);
            publisher.Override<NotificationTriggered>(Handle, ita);
            publisher.Extend<GradingChanged>(Handle, ita);
            publisher.Extend<RegistrationCancelled>(Handle, ita, true);
            publisher.Extend<RegistrationDeleted>(Handle, ita, true);

            _broker = new CommandBroker(commander);

            _da = da;
            _daStore = daStore;

            _warning = warning;
            _error = error;
            _mailgun = mailgun;

            _commands = commands;
            _events = events;
            _registrations = registrations;
            _registrationStore = registrationStore;
            _banks = assessments;
            _contacts = contacts;
            _groups = groups;

            _filePaths = filePaths;

            _domain = domain;
        }
    }
}