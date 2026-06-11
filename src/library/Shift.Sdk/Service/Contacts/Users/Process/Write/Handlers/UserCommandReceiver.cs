using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Users.Write;
using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Application.Contacts.Write
{
    public class UserCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IUserSearch _userSearch;
        private readonly IPersonSearch _personSearch;

        public UserCommandReceiver(
            ICommandQueue commander,
            IChangeQueue publisher,
            IChangeRepository repository,
            IUserSearch userSearch,
            IPersonSearch personSearch
            )
        {
            _publisher = publisher;
            _repository = repository;
            _userSearch = userSearch;
            _personSearch = personSearch;

            commander.Subscribe<CreateUser>(Handle);
            commander.Subscribe<DeleteUser>(Handle);
            commander.Subscribe<ModifyUserDefaultPassword>(Handle);
            commander.Subscribe<ModifyUserName>(Handle);
            commander.Subscribe<ModifyUserPassword>(Handle);
            commander.Subscribe<ArchiveUser>(Handle);
            commander.Subscribe<UnarchiveUser>(Handle);
            commander.Subscribe<ConnectUser>(Handle);
            commander.Subscribe<DisconnectUser>(Handle);
            commander.Subscribe<ModifyUserFieldText>(Handle);
            commander.Subscribe<ModifyUserFieldDateOffset>(Handle);
            commander.Subscribe<ModifyUserFieldBool>(Handle);
            commander.Subscribe<ModifyUserFieldInt>(Handle);
        }

        private void Commit(UserAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
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

        public void Handle(CreateUser c)
        {
            if (_userSearch.IsUserExist(c.AggregateIdentifier))
                throw new AggregateException($"UserIdentifier is already used: {c.AggregateIdentifier}");

            if (_userSearch.IsUserExist(c.Email))
                throw new AggregateException($"Email is already used: {c.Email}");

            if (string.IsNullOrEmpty(c.FirstName))
                throw new AggregateException($"FirstName is empty");

            if (string.IsNullOrEmpty(c.LastName))
                throw new AggregateException($"LastName is empty");

            if (string.IsNullOrEmpty(c.TimeZone))
                throw new AggregateException($"TimeZone is empty");

            var fullName = UserNameHelper.GetFullName(c.FullNamePolicy, c.FirstName, c.MiddleName, c.LastName, null);

            var aggregate = new UserAggregate { AggregateIdentifier = c.AggregateIdentifier };

            aggregate.CreateUser(c.Email, c.FirstName, c.LastName, c.MiddleName, fullName, c.TimeZone);

            if (c.MultiFactorAuthentication != aggregate.Data.GetBoolValue(UserField.MultiFactorAuthentication))
                aggregate.ModifyUserBoolField(UserField.MultiFactorAuthentication, c.MultiFactorAuthentication);

            Commit(aggregate, c);
        }

        public void Handle(DeleteUser c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteUser();
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyUserDefaultPassword c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (StringHelper.EqualsCaseSensitive(aggregate.Data.GetTextValue(UserField.DefaultPassword), c.DefaultPassword, true)
                    && aggregate.Data.GetDateOffsetValue(UserField.DefaultPasswordExpired) == c.DefaultPasswordExpired
                    )
                {
                    return;
                }

                aggregate.ModifyUserDefaultPassword(c.DefaultPassword, c.DefaultPasswordExpired);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyUserName c)
        {
            if (string.IsNullOrEmpty(c.FirstName))
                throw new AggregateException($"FirstName is empty");

            if (string.IsNullOrEmpty(c.LastName))
                throw new AggregateException($"LastName is empty");

            var fullName = UserNameHelper.GetFullName(c.FullNamePolicy, c.FirstName, c.MiddleName, c.LastName, null);
            var isUpdated = false;

            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;

                if (StringHelper.EqualsCaseSensitive(data.GetTextValue(UserField.FirstName), c.FirstName, true)
                    && StringHelper.EqualsCaseSensitive(data.GetTextValue(UserField.LastName), c.LastName, true)
                    && StringHelper.EqualsCaseSensitive(data.GetTextValue(UserField.MiddleName), c.MiddleName, true)
                    && StringHelper.EqualsCaseSensitive(data.GetTextValue(UserField.FullName), fullName, true)
                    )
                {
                    return;
                }

                aggregate.ModifyUserName(c.FirstName, c.LastName, c.MiddleName, fullName);
                Commit(aggregate, c);

                isUpdated = true;
            });

            if (isUpdated)
                RenamePersonName(c);
        }

        private void RenamePersonName(ModifyUserName c)
        {
            var filter = new QPersonFilter { UserIdentifier = c.AggregateIdentifier };
            var people = _personSearch.GetPersons(filter, x => x.Organization);

            foreach (var person in people)
            {
                var fullName = UserNameHelper.GetFullName(person.Organization.PersonFullNamePolicy, c.FirstName, c.MiddleName, c.LastName, person.EmployeeType);

                _repository.LockAndRun<PersonAggregate>(person.PersonIdentifier, aggregate =>
                {
                    var data = aggregate.Data;
                    if (StringHelper.EqualsCaseSensitive(data.GetTextValue(PersonField.FullName), fullName, true))
                        return;

                    aggregate.ModifyPersonFieldText(PersonField.FullName, fullName);
                    Commit(aggregate, c);
                });
            }
        }

        public void Handle(ModifyUserPassword c)
        {
            if (string.IsNullOrEmpty(c.PasswordHash))
                throw new AggregateException($"PasswordHash is empty");

            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;

                if (StringHelper.EqualsCaseSensitive(data.GetTextValue(UserField.UserPasswordHash), c.PasswordHash, true)
                    && data.GetDateOffsetValue(UserField.UserPasswordChanged) == c.PasswordChanged
                    && data.GetDateOffsetValue(UserField.UserPasswordExpired) == c.PasswordExpired
                    )
                {
                    return;
                }

                aggregate.ModifyUserPassword(c.PasswordHash, c.PasswordChanged, c.PasswordExpired);
                Commit(aggregate, c);
            });
        }

        public void Handle(ArchiveUser c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (data.GetDateOffsetValue(UserField.UtcArchived) == c.Date && data.GetDateOffsetValue(UserField.UtcUnarchived) == null)
                    return;

                aggregate.ArchiveUser(c.Date);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnarchiveUser c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                var data = aggregate.Data;
                if (data.GetDateOffsetValue(UserField.UtcUnarchived) == c.Date && data.GetDateOffsetValue(UserField.UtcArchived) == null)
                    return;

                aggregate.UnarchiveUser(c.Date);
                Commit(aggregate, c);
            });
        }

        public void Handle(ConnectUser c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (aggregate.Data.Connections.TryGetValue(c.ToUserId, out var conn)
                    && conn.IsLeader == c.IsLeader
                    && conn.IsManager == c.IsManager
                    && conn.IsSupervisor == c.IsSupervisor
                    && conn.IsValidator == c.IsValidator
                    && conn.Connected == c.Connected
                )
                {
                    return;
                }

                aggregate.ConnectUser(c.ToUserId, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected);
                Commit(aggregate, c);
            });
        }

        public void Handle(DisconnectUser c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (!aggregate.Data.Connections.ContainsKey(c.ToUserId))
                    return;

                aggregate.DisconnectUser(c.ToUserId);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyUserFieldText c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (StringHelper.EqualsCaseSensitive(aggregate.Data.GetTextValue(c.UserField), c.Value, true))
                    return;

                if (c.UserField == UserField.Email)
                {
                    var user = _userSearch.GetUserByEmail(c.Value);
                    if (user != null && user.UserIdentifier != c.AggregateIdentifier)
                        throw new ArgumentException($"There is a user with the same email: {c.Value}");
                }

                aggregate.ModifyUserTextField(c.UserField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyUserFieldDateOffset c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (aggregate.Data.GetDateOffsetValue(c.UserField) == c.Value)
                    return;

                aggregate.ModifyUserDateField(c.UserField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyUserFieldBool c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (aggregate.Data.GetBoolValue(c.UserField) == c.Value)
                    return;

                aggregate.ModifyUserBoolField(c.UserField, c.Value);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifyUserFieldInt c)
        {
            _repository.LockAndRun<UserAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (aggregate.Data.GetIntValue(c.UserField) == c.Value)
                    return;

                aggregate.ModifyUserIntField(c.UserField, c.Value);
                Commit(aggregate, c);
            });
        }
    }
}
