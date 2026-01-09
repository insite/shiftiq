using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Invoices.Read;
using InSite.Application.Invoices.Write;
using InSite.Application.Messages.Write;
using InSite.Application.Sales.Invoices.Write.Commands;
using InSite.Domain.Contacts;
using InSite.Domain.Invoices;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    /// <summary>
    /// Implements the processor for Membership changes.
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain changes 
    /// in a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes 
    /// purely reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a 
    /// state machine that is driven forward by incoming changes (which may come from many aggregates). Some states 
    /// will have side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class MembershipChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IGroupSearch _groups;
        private readonly IMembershipSearch _memberships;
        private readonly IContactSearch _users;
        private readonly IInvoiceSearch _invoices;

        private readonly IAlertMailer _mailer;
        private readonly Urls _urls;

        private readonly Func<Guid, int> _generateInvoiceNumber;

        public MembershipChangeProcessor(
            ICommander commander,
            IChangeQueue publisher,
            IContactSearch users,
            IGroupSearch groups,
            IMembershipSearch memberships,
            IInvoiceSearch invoices,
            IAlertMailer mailer,
            Urls urls,
            Func<Guid, int> generateInvoiceNumber
            )
        {
            _commander = commander;
            _groups = groups;
            _memberships = memberships;
            _users = users;
            _invoices = invoices;

            _mailer = mailer;
            _urls = urls;

            _generateInvoiceNumber = generateInvoiceNumber;

            publisher.Subscribe<MembershipEnded>(Handle);
            publisher.Subscribe<MembershipExpired>(Handle);
            publisher.Subscribe<MembershipStarted>(Handle);
            publisher.Subscribe<MembershipResumed>(Handle);
        }

        private void Handle(MembershipEnded ended)
        {
            var variables = GetVariables(ended.OriginUser, ended.AggregateIdentifier, null, null, "Ended");
            SendNotificationForMembershipEnded(variables);
        }

        private void Handle(MembershipExpired expired)
        {
            var variables = GetVariables(expired.OriginUser, expired.AggregateIdentifier, null, null, "Expired");
            SendNotificationForMembershipEnded(variables);
        }

        private void Handle(MembershipStarted started)
        {
            CreateInvoice(started.Group, started.User);

            var variables = GetVariables(started.OriginUser, started.AggregateIdentifier, started.Group, started.User, "Started");
            SendNotificationForMembershipStarted(variables);
        }

        private void Handle(MembershipResumed resumed)
        {
            CreateInvoice(resumed.Group, resumed.User);

            var variables = GetVariables(resumed.OriginUser, resumed.AggregateIdentifier, resumed.Group, resumed.User, "Resumed");
            SendNotificationForMembershipStarted(variables);
        }

        private void CreateInvoice(Guid groupId, Guid userId)
        {
            var group = _groups.GetGroup(groupId);
            if (group?.MembershipProductIdentifier == null)
                return;

            var productId = group.MembershipProductIdentifier.Value;
            var product = _invoices.GetProduct(productId);
            if (product == null)
                return;

            var person = _users.GetPerson(userId, group.OrganizationIdentifier);
            var invoiceId = UuidFactory.Create();

            var invoiceItems = new InvoiceItem[]
            {
                new InvoiceItem
                {
                    Identifier = UuidFactory.Create(),
                    Product = productId,
                    Quantity = 1,
                    Price = product.ProductPrice ?? 0,
                    Description = !string.IsNullOrEmpty(product.ProductDescription) ? product.ProductDescription : product.ProductName
                }
            };

            var invoiceNumber = _generateInvoiceNumber.Invoke(group.OrganizationIdentifier);

            var commands = new List<ICommand>
            {
                new DraftInvoice(invoiceId, group.OrganizationIdentifier, invoiceNumber, userId, invoiceItems)
            };

            if (person?.EmployerGroupIdentifier != null)
                commands.Add(new ChangeBusinessCustomer(invoiceId, person.EmployerGroupIdentifier));

            commands.Add(new SubmitInvoice(invoiceId));

            foreach (var command in commands)
                _commander.Send(command);
        }

        private void SendNotificationForMembershipEnded(Variables variables)
        {
            if (variables == null)
                return;

            var alert = new MembershipEndedNotification
            {
                OriginOrganization = variables.OriginOrganization,
                OriginUser = variables.OriginUser,

                GroupIdentifier = variables.GroupIdentifier,
                GroupName = variables.GroupName,

                UserIdentifier = variables.UserIdentifier,
                UserName = variables.UserName,
                UserEmail = variables.UserEmail,

                AppUrl = variables.AppUrl,
                EditPersonUrl = variables.EditPersonUrl,
                Reason = variables.Reason
            };

            if (variables.MessageToAdminWhenMembershipEnded.HasValue)
            {
                alert.MessageIdentifier = variables.MessageToAdminWhenMembershipEnded;
                _mailer.Send(alert, null);
            }

            if (variables.MessageToUserWhenMembershipEnded.HasValue)
            {
                alert.MessageIdentifier = variables.MessageToUserWhenMembershipEnded;
                _mailer.Send(alert, variables.UserIdentifier);
            }
        }

        private void SendNotificationForMembershipStarted(Variables variables)
        {
            if (variables == null)
                return;

            var alert = new MembershipStartedNotification
            {
                OriginOrganization = variables.OriginOrganization,
                OriginUser = variables.OriginUser,

                GroupIdentifier = variables.GroupIdentifier,
                GroupName = variables.GroupName,

                UserIdentifier = variables.UserIdentifier,
                UserName = variables.UserName,
                UserEmail = variables.UserEmail,

                AppUrl = variables.AppUrl,
                EditPersonUrl = variables.EditPersonUrl,
                Reason = variables.Reason
            };

            if (variables.MessageToAdminWhenMembershipStarted.HasValue)
            {
                alert.MessageIdentifier = variables.MessageToAdminWhenMembershipStarted;
                _mailer.Send(alert, null);
            }

            if (variables.MessageToUserWhenMembershipStarted.HasValue)
            {
                alert.MessageIdentifier = variables.MessageToUserWhenMembershipStarted;
                _mailer.Send(alert, variables.UserIdentifier);
            }
        }

        private Variables GetVariables(Guid actorId, Guid membershipId, Guid? groupId, Guid? userId, string reason)
        {
            var variables = new Variables();

            var membership = _memberships.Select(membershipId);
            if (membership != null)
            {
                membershipId = membership.MembershipIdentifier;
                groupId = membership.GroupIdentifier;
                userId = membership.UserIdentifier;
            }
            else
            {
                var deletion = _memberships.SelectDeletion(membershipId);
                if (deletion != null)
                {
                    membershipId = deletion.MembershipIdentifier;
                    groupId = deletion.GroupIdentifier;
                    userId = deletion.UserIdentifier;
                }
            }

            if (groupId == null || userId == null)
                return null;

            var group = _groups.GetGroup(groupId.Value);
            if (group == null)
                return null;

            var user = _users.GetUser(userId.Value);
            if (user == null)
                return null;

            variables.OrganizationCode = _groups.GetOrganizationCode(group.OrganizationIdentifier);
            variables.AppUrl = _urls.GetApplicationUrl(variables.OrganizationCode);

            variables.OriginOrganization = group.OrganizationIdentifier;
            variables.OriginUser = actorId;

            variables.GroupIdentifier = groupId.Value;
            variables.GroupName = group.GroupName;

            variables.MessageToAdminWhenMembershipEnded = group.MessageToAdminWhenMembershipEnded;
            variables.MessageToAdminWhenMembershipStarted = group.MessageToAdminWhenMembershipStarted;
            variables.MessageToUserWhenMembershipEnded = group.MessageToUserWhenMembershipEnded;
            variables.MessageToUserWhenMembershipStarted = group.MessageToUserWhenMembershipStarted;

            variables.UserIdentifier = userId.Value;
            variables.UserName = user.UserFullName;
            variables.UserEmail = user.UserEmail;

            variables.EditPersonUrl = $"{variables.AppUrl}/ui/admin/contacts/people/edit?contact={userId}";
            variables.Reason = reason;

            return variables;
        }

        private class Variables
        {
            public Guid OriginOrganization { get; internal set; }
            public Guid OriginUser { get; internal set; }

            public string OrganizationCode { get; internal set; }
            public string AppUrl { get; internal set; }
            public string EditPersonUrl { get; set; }

            public Guid GroupIdentifier { get; set; }
            public string GroupName { get; set; }

            public Guid UserIdentifier { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }

            public string Reason { get; set; }

            public Guid? MessageToAdminWhenMembershipEnded { get; internal set; }
            public Guid? MessageToAdminWhenMembershipStarted { get; internal set; }

            public Guid? MessageToUserWhenMembershipEnded { get; internal set; }
            public Guid? MessageToUserWhenMembershipStarted { get; internal set; }
        }
    }
}