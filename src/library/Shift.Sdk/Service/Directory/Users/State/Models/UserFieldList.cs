using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InSite.Domain.Contacts
{
    internal static class UserFieldList
    {
        private static readonly Dictionary<UserField, IStateFieldMeta> _fields = new Dictionary<UserField, IStateFieldMeta>
        {
            { UserField.DefaultPassword, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { UserField.Email, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true } },
            { UserField.EmailAlternate, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.EmailVerified, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.FirstName, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true, DirectlyModifiable = false } },
            { UserField.FullName, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true, DirectlyModifiable = false } },
            { UserField.Honorific, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.ImageUrl, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.Initials, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.LastName, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true, DirectlyModifiable = false } },
            { UserField.LoginOrganizationCode, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.MiddleName, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { UserField.MultiFactorAuthenticationCode, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.OAuthProviderUserId, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.OldUserPasswordHash, new StateFieldMeta { FieldType = StateFieldType.Text, DirectlyModifiable = false } },
            { UserField.PhoneMobile, new StateFieldMeta { FieldType = StateFieldType.Text } },
            { UserField.TimeZone, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true } },
            { UserField.UserPasswordHash, new StateFieldMeta { FieldType = StateFieldType.Text, Required = true, DirectlyModifiable = false } },

            { UserField.AccessGrantedToCmds, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },
            { UserField.MultiFactorAuthentication, new StateFieldMeta { FieldType = StateFieldType.Bool, Required = true } },

            { UserField.PrimaryLoginMethod, new StateFieldMeta { FieldType = StateFieldType.Int } },
            { UserField.SecondaryLoginMethod, new StateFieldMeta { FieldType = StateFieldType.Int } },
            { UserField.UserPasswordChangeRequested, new StateFieldMeta { FieldType = StateFieldType.Int } },

            { UserField.AccountCloaked, new StateFieldMeta { FieldType = StateFieldType.DateOffset } },
            { UserField.DefaultPasswordExpired, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { UserField.UserLicenseAccepted, new StateFieldMeta { FieldType = StateFieldType.DateOffset } },
            { UserField.UserPasswordChanged, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { UserField.UserPasswordExpired, new StateFieldMeta { FieldType = StateFieldType.DateOffset, Required = true, DirectlyModifiable = false } },
            { UserField.UtcArchived, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
            { UserField.UtcUnarchived, new StateFieldMeta { FieldType = StateFieldType.DateOffset, DirectlyModifiable = false } },
        };

        public static IStateFieldMeta GetField(UserField userField) => _fields[userField];

        public static IReadOnlyDictionary<UserField, IStateFieldMeta> GetAllFields() =>
            new ReadOnlyDictionary<UserField, IStateFieldMeta>(_fields);
    }
}
