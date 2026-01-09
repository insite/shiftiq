using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Users.Write;
using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Application.Contacts.Write
{
    public class UserCommandCreator
    {
        private readonly QUser _old;
        private readonly QUser _new;

        private List<Command> _result = new List<Command>();

        private UserCommandCreator(QUser oldUser, QUser newUser)
        {
            _old = oldUser;
            _new = newUser;
        }

        public static List<Command> Create(QUser oldUser, QUser newUser, string fullNamePolicy)
        {
            return new UserCommandCreator(oldUser, newUser).Create(fullNamePolicy);
        }

        private List<Command> Create(string fullNamePolicy)
        {
            AddCreateCommand(fullNamePolicy);
            AddRenameCommand(fullNamePolicy);
            AddPasswordCommands();
            AddArchiveCommand();
            AddOtherTextChanges();
            AddOtherIntChanges();
            AddOtherBoolChanges();
            AddOtherDateChanges();

            return _result;
        }

        private void AddCreateCommand(string fullNamePolicy)
        {
            if (_old == null)
                _result.Add(new CreateUser(_new.UserIdentifier, _new.Email, _new.FirstName, _new.LastName, _new.MiddleName, fullNamePolicy, _new.TimeZone, _new.MultiFactorAuthentication));
        }

        private void AddRenameCommand(string fullNamePolicy)
        {
            if (_old != null
                && (
                    !StringHelper.EqualsCaseSensitive(_old.FirstName, _new.FirstName, true)
                    || !StringHelper.EqualsCaseSensitive(_old.LastName, _new.LastName, true)
                    || !StringHelper.EqualsCaseSensitive(_old.MiddleName, _new.MiddleName, true)
                )
            )
            {
                _result.Add(new ModifyUserName(_new.UserIdentifier, _new.FirstName, _new.LastName, _new.MiddleName, fullNamePolicy));
            }
        }

        private void AddPasswordCommands()
        {
            if (!StringHelper.EqualsCaseSensitive(_old?.DefaultPassword, _new.DefaultPassword, true)
                || _old?.DefaultPasswordExpired != _new.DefaultPasswordExpired
                )
            {
                _result.Add(new ModifyUserDefaultPassword(_new.UserIdentifier, _new.DefaultPassword, _new.DefaultPasswordExpired));
            }

            if (!StringHelper.EqualsCaseSensitive(_old?.UserPasswordHash, _new.UserPasswordHash, true)
                || _old?.UserPasswordChanged != _new.UserPasswordChanged
                || _old?.UserPasswordExpired != _new.UserPasswordExpired
                )
            {
                _result.Add(new ModifyUserPassword(_new.UserIdentifier, _new.UserPasswordHash, _new.UserPasswordChanged, _new.UserPasswordExpired));
            }
        }

        private void AddArchiveCommand()
        {
            if (_old?.UtcArchived == _new.UtcArchived && _old?.UtcUnarchived == _new.UtcUnarchived)
                return;

            if (_new.UtcArchived.HasValue)
                _result.Add(new ArchiveUser(_new.UserIdentifier, _new.UtcArchived.Value));
            else if (_new.UtcUnarchived.HasValue)
                _result.Add(new UnarchiveUser(_new.UserIdentifier, _new.UtcUnarchived.Value));
        }

        private void AddOtherTextChanges()
        {
            if (_old != null && !StringHelper.EqualsCaseSensitive(_old.Email, _new.Email, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.Email, _new.Email));

            if (_old != null && !StringHelper.EqualsCaseSensitive(_old.TimeZone, _new.TimeZone, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.TimeZone, _new.TimeZone));

            if (!StringHelper.EqualsCaseSensitive(_old?.EmailVerified, _new.EmailVerified, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.EmailVerified, _new.EmailVerified));

            if (!StringHelper.EqualsCaseSensitive(_old?.Honorific, _new.Honorific, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.Honorific, _new.Honorific));

            if (!StringHelper.EqualsCaseSensitive(_old?.ImageUrl, _new.ImageUrl, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.ImageUrl, _new.ImageUrl));

            if (!StringHelper.EqualsCaseSensitive(_old?.Initials, _new.Initials, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.Initials, _new.Initials));

            if (!StringHelper.EqualsCaseSensitive(_old?.MultiFactorAuthenticationCode, _new.MultiFactorAuthenticationCode, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.MultiFactorAuthenticationCode, _new.MultiFactorAuthenticationCode));

            if (!StringHelper.EqualsCaseSensitive(_old?.EmailAlternate, _new.EmailAlternate, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.EmailAlternate, _new.EmailAlternate));

            if (!StringHelper.EqualsCaseSensitive(_old?.PhoneMobile, _new.PhoneMobile, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.PhoneMobile, _new.PhoneMobile));

            if (!StringHelper.EqualsCaseSensitive(_old?.OAuthProviderUserId, _new.OAuthProviderUserId, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.OAuthProviderUserId, _new.OAuthProviderUserId));

            if (!StringHelper.EqualsCaseSensitive(_old?.LoginOrganizationCode, _new.LoginOrganizationCode, true))
                _result.Add(new ModifyUserFieldText(_new.UserIdentifier, UserField.LoginOrganizationCode, _new.LoginOrganizationCode));
        }

        private void AddOtherIntChanges()
        {
            if (_old?.PrimaryLoginMethod != _new.PrimaryLoginMethod)
                _result.Add(new ModifyUserFieldInt(_new.UserIdentifier, UserField.PrimaryLoginMethod, _new.PrimaryLoginMethod));

            if (_old?.SecondaryLoginMethod != _new.SecondaryLoginMethod)
                _result.Add(new ModifyUserFieldInt(_new.UserIdentifier, UserField.SecondaryLoginMethod, _new.SecondaryLoginMethod));

            if (_old?.UserPasswordChangeRequested != _new.UserPasswordChangeRequested)
                _result.Add(new ModifyUserFieldInt(_new.UserIdentifier, UserField.UserPasswordChangeRequested, _new.UserPasswordChangeRequested));
        }

        private void AddOtherBoolChanges()
        {
            if (_old?.AccessGrantedToCmds != _new.AccessGrantedToCmds)
                _result.Add(new ModifyUserFieldBool(_new.UserIdentifier, UserField.AccessGrantedToCmds, _new.AccessGrantedToCmds));

            if (_old?.MultiFactorAuthentication != _new.MultiFactorAuthentication)
                _result.Add(new ModifyUserFieldBool(_new.UserIdentifier, UserField.MultiFactorAuthentication, _new.MultiFactorAuthentication));
        }

        private void AddOtherDateChanges()
        {
            if (_old?.AccountCloaked != _new.AccountCloaked)
                _result.Add(new ModifyUserFieldDateOffset(_new.UserIdentifier, UserField.AccountCloaked, _new.AccountCloaked));

            if (_old?.UserLicenseAccepted != _new.UserLicenseAccepted)
                _result.Add(new ModifyUserFieldDateOffset(_new.UserIdentifier, UserField.UserLicenseAccepted, _new.UserLicenseAccepted));
        }
    }
}
