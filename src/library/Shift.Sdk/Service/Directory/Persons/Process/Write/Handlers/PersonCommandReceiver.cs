using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Domain.Contacts;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Contacts.Write
{
    public class PersonCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IUserSearch _userSearch;
        private readonly IPersonSearch _personSearch;
        private readonly Func<Guid, string> _getFullNamePolicy;

        public PersonCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository,
            IUserSearch userSearch,
            IPersonSearch personSearch,
            Func<Guid, string> getFullNamePolicy
            )
        {
            _publisher = publisher;
            _repository = repository;
            _userSearch = userSearch;
            _personSearch = personSearch;
            _getFullNamePolicy = getFullNamePolicy;

            commander.Subscribe<CreatePerson>(Handle);
            commander.Subscribe<ArchivePerson>(Handle);
            commander.Subscribe<UnarchivePerson>(Handle);
            commander.Subscribe<DeletePerson>(Handle);
            commander.Subscribe<ModifyPersonAddress>(Handle);
            commander.Subscribe<ModifyPersonComment>(Handle);
            commander.Subscribe<ApprovePersonJob>(Handle);
            commander.Subscribe<GrantPersonAccess>(Handle);
            commander.Subscribe<RevokePersonAccess>(Handle);
            commander.Subscribe<FixPersonFieldDateOffset>(Handle);
            commander.Subscribe<ModifyPersonFieldText>(Handle);
            commander.Subscribe<ModifyPersonFieldDateOffset>(Handle);
            commander.Subscribe<ModifyPersonFieldDate>(Handle);
            commander.Subscribe<ModifyPersonFieldBool>(Handle);
            commander.Subscribe<ModifyPersonFieldInt>(Handle);
            commander.Subscribe<ModifyPersonFieldGuid>(Handle);
        }

        private void Commit(PersonAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(CreatePerson c)
        {
            var fullName = GetFullName(c.OrganizationId, c.UserId, null);

            var aggregate = new PersonAggregate { AggregateIdentifier = c.AggregateIdentifier, RootAggregateIdentifier = c.UserId };

            aggregate.CreatePerson(c.UserId, c.OrganizationId, fullName);

            Commit(aggregate, c);
        }

        public void Handle(DeletePerson c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeletePerson();
                Commit(aggregate, c);
            });
        }

        public void Handle(ArchivePerson c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ArchivePerson(c.Date);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnarchivePerson c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnarchivePerson(c.Date);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonAddress c)
        {
            var newAddress = c.Address != null && !c.Address.IsEmpty() ? c.Address : null;

            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                PersonAddress address;

                switch (c.AddressType)
                {
                    case AddressType.Billing:
                        address = aggregate.Data.BillingAddress;
                        break;
                    case AddressType.Shipping:
                        address = aggregate.Data.ShippingAddress;
                        break;
                    case AddressType.Work:
                        address = aggregate.Data.WorkAddress;
                        break;
                    case AddressType.Home:
                        address = aggregate.Data.HomeAddress;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported address type: {c.AddressType}");
                }

                if (address == null && newAddress == null
                    || address != null && newAddress != null
                        && StringHelper.EqualsCaseSensitive(address.City, newAddress.City, true)
                        && StringHelper.EqualsCaseSensitive(address.Country, newAddress.Country, true)
                        && StringHelper.EqualsCaseSensitive(address.Description, newAddress.Description, true)
                        && StringHelper.EqualsCaseSensitive(address.PostalCode, newAddress.PostalCode, true)
                        && StringHelper.EqualsCaseSensitive(address.Province, newAddress.Province, true)
                        && StringHelper.EqualsCaseSensitive(address.Street1, newAddress.Street1, true)
                        && StringHelper.EqualsCaseSensitive(address.Street2, newAddress.Street2, true)
                    )
                {
                    return;
                }

                if (newAddress != null)
                {
                    if (address != null)
                        newAddress.Identifier = address.Identifier;
                    else if (newAddress.Identifier == Guid.Empty)
                        newAddress.Identifier = UuidFactory.Create();
                }

                aggregate.ModifyPersonAddress(c.AddressType, newAddress);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonComment c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifyPersonComment(c.CommentActionType, c.Comment);
                Commit(aggregate, c);
            });
        }

        public void Handle(ApprovePersonJob c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Approved == data.GetDateOffsetValue(PersonField.JobsApproved)
                    && StringHelper.EqualsCaseSensitive(c.ApprovedBy, data.GetTextValue(PersonField.JobsApprovedBy), true)
                    )
                {
                    return;
                }

                aggregate.ApprovePersonJob(c.Approved, c.ApprovedBy);
                Commit(aggregate, c);
            });
        }

        public void Handle(GrantPersonAccess c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Granted == data.GetDateOffsetValue(PersonField.UserAccessGranted)
                    && StringHelper.EqualsCaseSensitive(c.GrantedBy, data.GetTextValue(PersonField.UserAccessGrantedBy), true)
                    && data.GetDateOffsetValue(PersonField.AccessRevoked) == null
                    && string.IsNullOrEmpty(data.GetTextValue(PersonField.AccessRevokedBy))
                    )
                {
                    return;
                }

                aggregate.GrantPersonAccess(c.Granted, c.GrantedBy);
                Commit(aggregate, c);
            });
        }

        public void Handle(RevokePersonAccess c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Revoked == data.GetDateOffsetValue(PersonField.AccessRevoked)
                    && StringHelper.EqualsCaseSensitive(c.RevokedBy, data.GetTextValue(PersonField.AccessRevokedBy), true)
                    && data.GetDateOffsetValue(PersonField.UserAccessGranted) == null
                    && string.IsNullOrEmpty(data.GetTextValue(PersonField.UserAccessGrantedBy))
                    )
                {
                    return;
                }

                aggregate.RevokePersonAccess(c.Revoked, c.RevokedBy);
                Commit(aggregate, c);
            });
        }

        public void Handle(FixPersonFieldDateOffset c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Value == data.GetDateOffsetValue(c.PersonField))
                    return;

                aggregate.FixPersonFieldDateOffset(c.PersonField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonFieldText c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (StringHelper.EqualsCaseSensitive(c.Value, data.GetTextValue(c.PersonField), true))
                    return;

                if (c.PersonField == PersonField.PersonCode
                    && !string.IsNullOrEmpty(c.Value)
                    )
                {
                    var persons = _personSearch.GetPersonsByPersonCodes(new[] { c.Value }, data.Organization);
                    if (persons.Count > 0 && persons[0].PersonIdentifier != c.AggregateIdentifier)
                        throw new ArgumentException($"There is a person in the org {data.Organization} with the same code: {c.Value}");
                }

                aggregate.ModifyPersonFieldText(c.PersonField, c.Value);

                if (c.PersonField == PersonField.EmployeeType)
                {
                    var fullName = GetFullName(data.Organization, data.User, c.Value);
                    if (!StringHelper.EqualsCaseSensitive(fullName, data.GetTextValue(PersonField.FullName), true))
                        aggregate.ModifyPersonFieldText(PersonField.FullName, fullName);
                }

                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonFieldDateOffset c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Value == data.GetDateOffsetValue(c.PersonField))
                    return;

                aggregate.ModifyPersonFieldDateOffset(c.PersonField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonFieldDate c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Value == data.GetDateValue(c.PersonField))
                    return;

                aggregate.ModifyPersonFieldDate(c.PersonField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonFieldBool c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Value == data.GetBoolValue(c.PersonField))
                    return;

                aggregate.ModifyPersonFieldBool(c.PersonField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonFieldInt c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Value == data.GetIntValue(c.PersonField))
                    return;

                aggregate.ModifyPersonFieldInt(c.PersonField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyPersonFieldGuid c)
        {
            _repository.LockAndRun<PersonAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (c.Value == data.GetGuidValue(c.PersonField))
                    return;

                aggregate.ModifyPersonFieldGuid(c.PersonField, c.Value);
                Commit(aggregate, c);
            });
        }

        private string GetFullName(Guid organizationId, Guid userId, string employeeType)
        {
            var user = _userSearch.GetUser(userId);
            var fullNamePolicy = _getFullNamePolicy(organizationId);

            return UserNameHelper.GetFullName(fullNamePolicy, user?.FirstName, user?.MiddleName, user?.LastName, employeeType);
        }
    }
}
