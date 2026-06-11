using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    [Serializable]
    public class UserState : AggregateState
    {
        public static class Defaults
        {
            public static readonly bool AccessGrantedToCmds = false;
            public static readonly bool MultiFactorAuthentication = false;
            public static readonly string UserPasswordHash = "none";
            public static readonly DateTimeOffset UserPasswordExpired = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
        }

        [JsonProperty, JsonConverter(typeof(UserStateDictionaryConverter))]
        private Dictionary<UserField, object> Values = new Dictionary<UserField, object>();

        public Guid Identifier { get; set; }

        // Key is ToUser
        public Dictionary<Guid, UserConnection> Connections { get; set; } = new Dictionary<Guid, UserConnection>();

        internal object GetValue(UserField userField) => Values.TryGetValue(userField, out var value) ? value : null;

        public string GetTextValue(UserField userField) => Values.TryGetValue(userField, out var value) ? (string)value : null;
        public bool? GetBoolValue(UserField userField) => Values.TryGetValue(userField, out var value) ? (bool?)value : null;
        public int? GetIntValue(UserField userField) => Values.TryGetValue(userField, out var value) ? (int?)value : null;
        public DateTimeOffset? GetDateOffsetValue(UserField userField) => Values.TryGetValue(userField, out var value) ? (DateTimeOffset?)value : null;

        private void SetValue<T>(UserField userField, T value, bool directlyModified)
        {
            var field = UserFieldList.GetField(userField);
            if (value != null && field.FieldType != StateFieldHelper.GetFieldType<T>())
                throw new ArgumentException($"Invalid user field: {userField}");

            if (directlyModified && !field.DirectlyModifiable)
                throw new ArgumentException($"The field cannot be modified directly field: {userField}");

            if (value == null)
            {
                if (field.Required)
                    throw new ArgumentNullException($"Field {userField} is a required field");

                Values.Remove(userField);
            }
            else
                Values[userField] = value;
        }

        public void When(UserCreated e)
        {
            Identifier = e.AggregateIdentifier;
            SetValue(UserField.Email, e.Email, false);
            SetValue(UserField.FirstName, e.FirstName, false);
            SetValue(UserField.LastName, e.LastName, false);
            SetValue(UserField.MiddleName, e.MiddleName, false);
            SetValue(UserField.FullName, e.FullName, false);
            SetValue(UserField.TimeZone, e.TimeZone, false);
            SetValue(UserField.AccessGrantedToCmds, Defaults.AccessGrantedToCmds, false);
            SetValue(UserField.MultiFactorAuthentication, Defaults.MultiFactorAuthentication, false);
            SetValue(UserField.UserPasswordHash, Defaults.UserPasswordHash, false);
            SetValue(UserField.UserPasswordExpired, Defaults.UserPasswordExpired, false);
        }

        public void When(UserDeleted _)
        {
        }

        public void When(UserDefaultPasswordModified e)
        {
            SetValue(UserField.DefaultPassword, e.DefaultPassword, false);
            SetValue(UserField.DefaultPasswordExpired, e.DefaultPasswordExpired, false);
        }

        public void When(UserNameModified e)
        {
            SetValue(UserField.FirstName, e.FirstName, false);
            SetValue(UserField.LastName, e.LastName, false);
            SetValue(UserField.MiddleName, e.MiddleName, false);
            SetValue(UserField.FullName, e.FullName, false);
        }

        public void When(UserPasswordModified e)
        {
            SetValue(UserField.OldUserPasswordHash, GetTextValue(UserField.UserPasswordHash), false);
            SetValue(UserField.UserPasswordHash, e.PasswordHash, false);
            SetValue(UserField.UserPasswordChanged, e.PasswordChanged, false);
            SetValue(UserField.UserPasswordExpired, e.PasswordExpired, false);
        }

        public void When(UserArchived e)
        {
            SetValue(UserField.UtcUnarchived, (DateTimeOffset?)null, false);
            SetValue(UserField.UtcArchived, e.Date, false);
        }

        public void When(UserUnarchived e)
        {
            SetValue(UserField.UtcArchived, (DateTimeOffset?)null, false);
            SetValue(UserField.UtcUnarchived, e.Date, false);
        }

        public void When(UserConnected e)
        {
            Connections[e.ToUserId] = new UserConnection
            {
                ToUser = e.ToUserId,
                IsLeader = e.IsLeader,
                IsManager = e.IsManager,
                IsSupervisor = e.IsSupervisor,
                IsValidator = e.IsValidator,
                Connected = e.Connected
            };
        }

        public void When(UserDisconnected e)
        {
            Connections.Remove(e.ToUserId);
        }

        public void When(UserFieldTextModified e)
        {
            SetValue(e.UserField, e.Value, true);
        }

        public void When(UserFieldDateOffsetModified e)
        {
            SetValue(e.UserField, e.Value, true);
        }

        public void When(UserFieldBoolModified e)
        {
            SetValue(e.UserField, e.Value, true);
        }

        public void When(UserFieldIntModified e)
        {
            SetValue(e.UserField, e.Value, true);
        }
    }
}
