using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    /// <summary>
    /// Implements the projector for User changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A 
    /// processor, in contrast, should *never* replay past changes.
    /// </remarks>
    public class UserChangeProjector
    {
        private readonly IUserStore _store;

        public UserChangeProjector(IChangeQueue publisher, IUserStore store)
        {
            _store = store;

            publisher.Subscribe<UserCreated>(Handle);
            publisher.Subscribe<UserDeleted>(Handle);
            publisher.Subscribe<UserDefaultPasswordModified>(Handle);
            publisher.Subscribe<UserNameModified>(Handle);
            publisher.Subscribe<UserPasswordModified>(Handle);
            publisher.Subscribe<UserArchived>(Handle);
            publisher.Subscribe<UserUnarchived>(Handle);
            publisher.Subscribe<UserConnected>(Handle);
            publisher.Subscribe<UserDisconnected>(Handle);
            publisher.Subscribe<UserFieldTextModified>(Handle);
            publisher.Subscribe<UserFieldDateOffsetModified>(Handle);
            publisher.Subscribe<UserFieldBoolModified>(Handle);
            publisher.Subscribe<UserFieldIntModified>(Handle);
        }

        public void Handle(UserCreated e)
            => _store.Insert(e);

        public void Handle(UserDeleted e)
            => _store.Delete(e);

        public void Handle(UserDefaultPasswordModified e)
            => _store.Update(e, x =>
            {
                x.DefaultPassword = e.DefaultPassword;
                x.DefaultPasswordExpired = e.DefaultPasswordExpired;
            });

        public void Handle(UserNameModified e)
            => _store.Update(e, x =>
            {
                x.FirstName = e.FirstName;
                x.LastName = e.LastName;
                x.MiddleName = e.MiddleName;
                x.FullName = e.FullName;
                x.SoundexFirstName = Pronunciation.Soundex(e.FirstName, 4, 0);
                x.SoundexLastName = Pronunciation.Soundex(e.LastName, 4, 0);
            });

        public void Handle(UserPasswordModified e)
            => _store.Update(e, x =>
            {
                x.OldUserPasswordHash = x.UserPasswordHash;
                x.UserPasswordHash = e.PasswordHash;
                x.UserPasswordChanged = e.PasswordChanged;
                x.UserPasswordExpired = e.PasswordExpired;
            });

        public void Handle(UserArchived e)
            => _store.Update(e, x =>
            {
                x.UtcUnarchived = null;
                x.UtcArchived = e.Date;
            });

        public void Handle(UserUnarchived e)
            => _store.Update(e, x =>
            {
                x.UtcArchived = null;
                x.UtcUnarchived = e.Date;
            });

        public void Handle(UserConnected e)
            => _store.Update(e);

        public void Handle(UserDisconnected e)
            => _store.Update(e);

        // Aleksey:
        // Whoever will decide to refactor this, please do not change to reflection and etc
        // Lets keep it simple
        public void Handle(UserFieldTextModified e)
            => _store.Update(e, x =>
            {
                switch (e.UserField)
                {
                    case UserField.Email:
                        x.Email = e.Value;
                        break;
                    case UserField.EmailAlternate:
                        x.EmailAlternate = e.Value;
                        break;
                    case UserField.EmailVerified:
                        x.EmailVerified = e.Value;
                        break;
                    case UserField.Honorific:
                        x.Honorific = e.Value;
                        break;
                    case UserField.ImageUrl:
                        x.ImageUrl = e.Value;
                        break;
                    case UserField.Initials:
                        x.Initials = e.Value;
                        break;
                    case UserField.LoginOrganizationCode:
                        x.LoginOrganizationCode = e.Value;
                        break;
                    case UserField.MultiFactorAuthenticationCode:
                        x.MultiFactorAuthenticationCode = e.Value;
                        break;
                    case UserField.OAuthProviderUserId:
                        x.OAuthProviderUserId = e.Value;
                        break;
                    case UserField.PhoneMobile:
                        x.PhoneMobile = e.Value;
                        break;
                    case UserField.TimeZone:
                        x.TimeZone = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported text user field: {e.UserField}");
                }
            });

        // Aleksey:
        // Whoever will decide to refactor this, please do not change to reflection and etc
        // Lets keep it simple
        public void Handle(UserFieldBoolModified e)
            => _store.Update(e, x =>
            {
                switch (e.UserField)
                {
                    case UserField.AccessGrantedToCmds:
                        x.AccessGrantedToCmds = e.Value.Value;
                        break;
                    case UserField.MultiFactorAuthentication:
                        x.MultiFactorAuthentication = e.Value.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported bool user field: {e.UserField}");
                }
            });

        // Aleksey:
        // Whoever will decide to refactor this, please do not change to reflection and etc
        // Lets keep it simple
        public void Handle(UserFieldIntModified e)
            => _store.Update(e, x =>
            {
                switch (e.UserField)
                {
                    case UserField.PrimaryLoginMethod:
                        x.PrimaryLoginMethod = e.Value;
                        break;
                    case UserField.SecondaryLoginMethod:
                        x.SecondaryLoginMethod = e.Value;
                        break;
                    case UserField.UserPasswordChangeRequested:
                        x.UserPasswordChangeRequested = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported int user field: {e.UserField}");
                }
            });

        // Aleksey:
        // Whoever will decide to refactor this, please do not change to reflection and etc
        // Lets keep it simple
        public void Handle(UserFieldDateOffsetModified e)
            => _store.Update(e, x =>
            {
                switch (e.UserField)
                {
                    case UserField.AccountCloaked:
                        x.AccountCloaked = e.Value;
                        break;
                    case UserField.UserLicenseAccepted:
                        x.UserLicenseAccepted = e.Value;
                        break;
                    case UserField.UserPasswordChanged:
                        x.UserPasswordChanged = e.Value;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported date offset user field: {e.UserField}");
                }
            });

        /// <summary>
        /// Regenerate the projection of user changes from the log to query tables.
        /// </summary>
        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear existing data from the query tables.
            if (id.HasValue)
                _store.DeleteAll(id.Value);
            else
                _store.DeleteAll();

            // Get the subset of changes for which this projector is a subscriber. 
            var changes = store.GetChanges("User", id, null);

            // Handle each of the changes in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("User", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}
