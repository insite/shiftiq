using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    [Serializable]
    public class PersonState : AggregateState
    {
        public static class Defaults
        {
            public const bool IsAdministrator = false;
            public const bool IsDeveloper = false;
            public const bool IsLearner = false;
            public const bool IsOperator = false;
            public const bool EmailEnabled = false;
            public const bool EmailAlternateEnabled = false;
            public const bool MarketingEmailEnabled = true;
        }

        [JsonProperty, JsonConverter(typeof(PersonStateDictionaryConverter))]
        private Dictionary<PersonField, object> Values = new Dictionary<PersonField, object>();

        public Guid Identifier { get; set; }
        public Guid Organization { get; set; }
        public Guid User { get; set; }

        public PersonAddress BillingAddress { get; set; }
        public PersonAddress ShippingAddress { get; set; }
        public PersonAddress WorkAddress { get; set; }
        public PersonAddress HomeAddress { get; set; }
        public Dictionary<Guid, PersonComment> Comments { get; set; } = new Dictionary<Guid, PersonComment>();

        internal object GetValue(PersonField personField) => Values.TryGetValue(personField, out var value) ? value : null;

        public string GetTextValue(PersonField personField) => Values.TryGetValue(personField, out var value) ? (string)value : null;
        public bool? GetBoolValue(PersonField personField) => Values.TryGetValue(personField, out var value) ? (bool?)value : null;
        public int? GetIntValue(PersonField personField) => Values.TryGetValue(personField, out var value) ? (int?)value : null;
        public DateTimeOffset? GetDateOffsetValue(PersonField personField) => Values.TryGetValue(personField, out var value) ? (DateTimeOffset?)value : null;
        public DateTime? GetDateValue(PersonField personField) => Values.TryGetValue(personField, out var value) ? (DateTime?)value : null;
        public Guid? GetGuidValue(PersonField personField) => Values.TryGetValue(personField, out var value) ? (Guid?)value : null;

        private void SetValue<T>(PersonField personField, T value, bool directlyModified)
        {
            if (personField.IsObsolete())
                return;

            var field = PersonFieldList.GetField(personField);
            if (value != null && field.FieldType != StateFieldHelper.GetFieldType<T>())
                throw new ArgumentException($"Invalid person field: {personField}");

            if (directlyModified && !field.DirectlyModifiable)
                throw new ArgumentException($"The field cannot be modified directly field: {personField}");

            if (value == null)
            {
                if (field.Required)
                    throw new ArgumentNullException($"Field {personField} is a required field");

                Values.Remove(personField);
            }
            else
                Values[personField] = value;
        }

        public void When(PersonCreated e)
        {
            Identifier = e.AggregateIdentifier;
            User = e.UserId;
            Organization = e.OrganizationId;

            SetValue(PersonField.IsAdministrator, Defaults.IsAdministrator, false);
            SetValue(PersonField.IsLearner, Defaults.IsLearner, false);
            SetValue(PersonField.IsOperator, Defaults.IsOperator, false);
            SetValue(PersonField.EmailEnabled, Defaults.EmailEnabled, false);
            SetValue(PersonField.EmailAlternateEnabled, Defaults.EmailAlternateEnabled, false);
            SetValue(PersonField.MarketingEmailEnabled, Defaults.MarketingEmailEnabled, false);
            SetValue(PersonField.Created, e.ChangeTime, false);
            SetValue(PersonField.FullName, e.FullName, false);
        }

        public void When(PersonArchived e)
        {
            SetValue(PersonField.WhenUnarchived, (DateTimeOffset?)null, false);
            SetValue(PersonField.WhenArchived, e.Date, false);
        }

        public void When(PersonUnarchived e)
        {
            SetValue(PersonField.WhenArchived, (DateTimeOffset?)null, false);
            SetValue(PersonField.WhenUnarchived, e.Date, false);
        }

        public void When(PersonDeleted _)
        {
        }

        public void When(PersonAddressModified e)
        {
            switch (e.AddressType)
            {
                case AddressType.Billing:
                    BillingAddress = e.Address?.Clone();
                    break;
                case AddressType.Shipping:
                    ShippingAddress = e.Address?.Clone();
                    break;
                case AddressType.Home:
                    HomeAddress = e.Address?.Clone();
                    break;
                case AddressType.Work:
                    WorkAddress = e.Address?.Clone();
                    break;
                default:
                    throw new ArgumentException($"Unsupported address type: {e.AddressType}");
            }
        }

        public void When(PersonCommentModified e)
        {
            switch (e.CommentActionType)
            {
                case CommentActionType.Author:
                    e.Comment.Posted = e.ChangeTime;
                    e.Comment.Revised = e.ChangeTime;
                    Comments[e.Comment.Comment] = e.Comment.Clone();
                    break;
                case CommentActionType.Revise:
                    e.Comment.Posted = Comments.TryGetValue(e.Comment.Comment, out var existing) ? existing.Posted : e.ChangeTime;
                    e.Comment.Revised = e.ChangeTime;
                    Comments[e.Comment.Comment] = e.Comment.Clone();
                    break;
                case CommentActionType.Delete:
                    if (!Comments.Remove(e.Comment.Comment))
                        throw new KeyNotFoundException($"Cannot delete comment {e.Comment.Comment}: not found.");
                    break;
                default:
                    throw new ArgumentException($"Unsupported comment type: {e.CommentActionType}");
            }
        }

        public void When(PersonJobApproved e)
        {
            SetValue(PersonField.JobsApproved, e.Approved, false);
            SetValue(PersonField.JobsApprovedBy, e.ApprovedBy, false);
        }

        public void When(PersonAccessGranted e)
        {
            SetValue(PersonField.UserAccessGranted, e.Granted, false);
            SetValue(PersonField.UserAccessGrantedBy, e.GrantedBy, false);
            SetValue(PersonField.AccessRevoked, (DateTimeOffset?)null, false);
            SetValue(PersonField.AccessRevokedBy, (string)null, false);
        }

        public void When(PersonAccessRevoked e)
        {
            SetValue(PersonField.AccessRevoked, e.Revoked, false);
            SetValue(PersonField.AccessRevokedBy, e.RevokedBy, false);
            SetValue(PersonField.UserAccessGranted, (DateTimeOffset?)null, false);
            SetValue(PersonField.UserAccessGrantedBy, (string)null, false);
        }

        public void When(PersonFieldTextModified e)
        {
            SetValue(e.PersonField, e.Value, true);

            if (e.PersonField == PersonField.SocialInsuranceNumber)
                SetValue(PersonField.SinModified, e.ChangeTime, false);
        }

        public void When(PersonFieldDateOffsetFixed e)
        {
            SetValue(e.PersonField, e.Value, false);
        }

        public void When(PersonFieldDateOffsetModified e)
        {
            SetValue(e.PersonField, e.Value, true);
        }

        public void When(PersonFieldDateModified e)
        {
            SetValue(e.PersonField, e.Value, true);
        }

        public void When(PersonFieldBoolModified e)
        {
            SetValue(e.PersonField, e.Value, true);
        }

        public void When(PersonFieldIntModified e)
        {
            SetValue(e.PersonField, e.Value, true);
        }

        public void When(PersonFieldGuidModified e)
        {
            SetValue(e.PersonField, e.Value, true);
        }
    }
}
