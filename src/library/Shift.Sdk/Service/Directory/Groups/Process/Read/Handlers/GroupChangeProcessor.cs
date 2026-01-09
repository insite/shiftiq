using System;

using Shift.Common.Timeline.Changes;

using InSite.Application.Messages.Write;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;

using Shift.Constant;

namespace InSite.Application.Contacts.Read
{
    /// <summary>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain changes in a cross-aggregate, 
    /// eventually consistent manner. Time can be a trigger. Process managers are sometimes purely reactive, and sometimes represent workflows. From 
    /// an implementation perspective, a process manager is a state machine that is driven forward by incoming changes (which may come from many 
    /// aggregates). Some states will have side effects, such as sending commands, talking to external web services, or sending emails.
    /// </summary>
    public class GroupChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IAlertMailer _alertMailer;

        public GroupChangeProcessor(ICommander commander, IChangeQueue publisher, IAlertMailer alertMailer)
        {
            _commander = commander;
            _alertMailer = alertMailer;

            publisher.Subscribe<GroupCreated>(Handle);
        }

        public void Handle(GroupCreated c)
        {
            var isEmployer = string.Equals(c.Type, "Employer", StringComparison.OrdinalIgnoreCase);
            var isSomeone = c.OriginUser == UserIdentifiers.Someone;
            var isMaintenance = c.OriginUser == UserIdentifiers.Maintenance;

            if (!isEmployer || isSomeone || isMaintenance)
                return;

            _alertMailer.Send(c.Tenant, c.OriginUser, new AlertEmployerGroupCreated
            {
                GroupIdentifier = c.AggregateIdentifier,
                GroupName = c.Name
            });
        }
    }
}
