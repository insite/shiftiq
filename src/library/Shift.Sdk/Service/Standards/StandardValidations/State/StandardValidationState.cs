using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant.CMDS;

namespace InSite.Domain.Standards
{
    [Serializable]
    public class StandardValidationState : AggregateState
    {
        #region Properties

        [JsonProperty]
        public bool IsExist { get; private set; }

        #endregion

        #region Fields

        [JsonProperty(PropertyName = "Values"), JsonConverter(typeof(StandardValidationStateDictionaryConverter))]
        private Dictionary<StandardValidationField, object> _values = new Dictionary<StandardValidationField, object>();

        [JsonProperty(PropertyName = "Logs")]
        private Dictionary<Guid, StandardValidationLog> _logs = new Dictionary<Guid, StandardValidationLog>();

        #endregion

        #region Methods (Values)

        public object GetFieldValue(StandardValidationField field) => _values.GetOrDefault(field);

        public bool? GetBoolValue(StandardValidationField field) => (bool?)_values.GetOrDefault(field);

        public DateTimeOffset? GetDateOffsetValue(StandardValidationField field) => (DateTimeOffset?)_values.GetOrDefault(field);

        public Guid? GetGuidValue(StandardValidationField field) => (Guid?)_values.GetOrDefault(field);

        public string GetTextValue(StandardValidationField field) => (string)_values.GetOrDefault(field);

        private void SetValue<T>(StandardValidationField field, T value, bool directlyModified)
        {
            StandardValidationFieldList.Validate(field, value, directlyModified);

            if (value == null)
                _values.Remove(field);
            else
                _values[field] = value;
        }

        private void SetValue(StandardValidationField field, object value, bool directlyModified)
        {
            StandardValidationFieldList.Validate(field, value, directlyModified);

            if (value == null)
                _values.Remove(field);
            else
                _values[field] = value;
        }

        #endregion

        #region Methods (When)

        public void When(StandardValidationCreated e)
        {
            IsExist = true;
            SetValue(StandardValidationField.OrganizationIdentifier, e.OriginOrganization, false);
            SetValue(StandardValidationField.StandardIdentifier, e.StandardId, false);
            SetValue(StandardValidationField.UserIdentifier, e.UserId, false);
            SetValue(StandardValidationField.ValidationStatus, ValidationStatuses.NotCompleted, false);
            SetValue(StandardValidationField.IsValidated, false, false);
            SetValue(StandardValidationField.Created, e.ChangeTime, false);
            SetValue(StandardValidationField.CreatedBy, e.OriginUser, false);

            SetModified(e);
        }

        public void When(StandardValidationRemoved e)
        {
            IsExist = false;
        }

        public void When(StandardValidationTimestampsModified e)
        {
            SetValue(StandardValidationField.Created, e.Created, false);
            SetValue(StandardValidationField.CreatedBy, e.CreatedBy, false);
            SetValue(StandardValidationField.Modified, e.Modified, false);
            SetValue(StandardValidationField.ModifiedBy, e.ModifiedBy, false);
        }

        public void When(StandardValidationFieldTextModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardValidationFieldDateOffsetModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardValidationFieldBoolModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardValidationFieldGuidModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardValidationFieldsModified e)
        {
            foreach (var kv in e.Fields)
                SetValue(kv.Key, kv.Value, true);

            SetModified(e);
        }

        public void When(StandardValidationSelfValidated e)
        {
            var isDateApplicable = StringHelper.Equals(e.Status, SelfAssessedStatuses.SelfAssessed)
                || StringHelper.Equals(e.Status, SelfAssessedStatuses.NotApplicable);
            var date = isDateApplicable ? e.ChangeTime : (DateTimeOffset?)null;
            var oldValidationStatus = GetTextValue(StandardValidationField.ValidationStatus);
            var newValidationStatus = StatusMapping
                .Where(x => x.SelfValidationStatus == e.Status).Select(x => x.ValidationStatus).FirstOrDefault()
                .IfNullOrEmpty(ValidationStatuses.NotCompleted);

            SetValue(StandardValidationField.SelfAssessmentStatus, e.Status, false);
            SetValue(StandardValidationField.SelfAssessmentDate, date, false);
            SetValue(StandardValidationField.Expired, (DateTimeOffset?)null, false);
            SetValue(StandardValidationField.Notified, (DateTimeOffset?)null, false);
            SetValue(StandardValidationField.ValidationDate, (DateTimeOffset?)null, false);
            SetValue(StandardValidationField.IsValidated, false, false);
            SetValue(StandardValidationField.ValidatorUserIdentifier, (Guid?)null, false);
            SetValue(StandardValidationField.ValidationComment, (string)null, false);
            SetValue(StandardValidationField.ValidationStatus, newValidationStatus, false);
            SetModified(e);

            if (oldValidationStatus.IsNotEmpty() && !StringHelper.Equals(oldValidationStatus, newValidationStatus))
                AddLog(e, e.LogId, newValidationStatus, null);
        }

        public void When(StandardValidationStatusModified e)
        {
            var oldValidationStatus = GetTextValue(StandardValidationField.ValidationStatus);

            DateTimeOffset? selfAssessmentDate = null;
            if (StringHelper.Equals(e.SelfAssessmentStatus, SelfAssessedStatuses.SelfAssessed) || StringHelper.Equals(e.SelfAssessmentStatus, SelfAssessedStatuses.NotApplicable))
                selfAssessmentDate = e.ChangeTime;

            DateTimeOffset? validationDate = null;
            Guid? validatorUserIdentifier = null;
            if (e.ValidationStatus == ValidationStatuses.NotApplicable || e.ValidationStatus == ValidationStatuses.Validated)
            {
                validationDate = e.ChangeTime;
                validatorUserIdentifier = e.OriginUser;
            }

            SetValue(StandardValidationField.SelfAssessmentStatus, e.SelfAssessmentStatus, false);
            SetValue(StandardValidationField.SelfAssessmentDate, selfAssessmentDate, false);
            SetValue(StandardValidationField.Expired, (DateTimeOffset?)null, false);
            SetValue(StandardValidationField.Notified, (DateTimeOffset?)null, false);
            SetValue(StandardValidationField.ValidationDate, validationDate, false);
            SetValue(StandardValidationField.IsValidated, e.IsValidated, false);
            SetValue(StandardValidationField.ValidatorUserIdentifier, validatorUserIdentifier, false);
            SetValue(StandardValidationField.ValidationComment, e.ValidationComment, false);
            SetValue(StandardValidationField.ValidationStatus, e.ValidationStatus, false);
            SetModified(e);

            if (oldValidationStatus.IsNotEmpty() && !StringHelper.Equals(oldValidationStatus, e.ValidationStatus))
                AddLog(e, e.LogId, e.ValidationStatus, null);
        }

        public void When(StandardValidationSubmittedForValidation e)
        {
            var validationStatus = ValidationStatuses.SubmittedForValidation;

            SetValue(StandardValidationField.ValidationStatus, validationStatus, false);
            SetModified(e);

            AddLog(e, e.LogId, validationStatus, null);
        }

        public void When(StandardValidationValidated e)
        {
            if (e.IsValidated)
            {
                SetValue(StandardValidationField.ValidationDate, e.ChangeTime, false);
                SetValue(StandardValidationField.ValidatorUserIdentifier, e.OriginUser, false);
            }
            else
            {
                var selfStatus = StatusMapping
                    .Where(x => x.ValidationStatus == e.Status).Select(x => x.SelfValidationStatus).FirstOrDefault()
                    .IfNullOrEmpty(SelfAssessedStatuses.NotCompleted);

                SetValue(StandardValidationField.ValidatorUserIdentifier, null, false);
                SetValue(StandardValidationField.SelfAssessmentStatus, selfStatus, false);
            }

            SetValue(StandardValidationField.IsValidated, e.IsValidated, false);
            SetValue(StandardValidationField.ValidationStatus, e.Status, false);
            SetValue(StandardValidationField.ValidationComment, e.Comment, false);
            SetModified(e);

            AddLog(e, e.LogId, e.Status, e.Comment);
        }

        public void When(StandardValidationExpired e)
        {
            SetValue(StandardValidationField.Expired, e.ChangeTime, false);
            SetValue(StandardValidationField.Notified, null, false);
            SetValue(StandardValidationField.ValidationStatus, ValidationStatuses.Expired, false);
            SetValue(StandardValidationField.SelfAssessmentStatus, SelfAssessedStatuses.Expired, false);
            SetValue(StandardValidationField.SelfAssessmentDate, null, false);
            SetModified(e);

            AddLog(e, e.LogId, ValidationStatuses.Expired, e.Comment);
        }

        public void When(StandardValidationNotified e)
        {
            SetValue(StandardValidationField.Notified, e.Date ?? e.ChangeTime, false);
            SetModified(e);
        }

        public void When(StandardValidationLogAdded e)
        {
            foreach (var log in e.Logs)
            {
                if (!log.AuthorUserId.HasValue)
                    log.AuthorUserId = e.OriginUser;

                if (!log.Posted.HasValue)
                    log.Posted = e.ChangeTime;

                _logs.Add(log.LogId, log);
            }
        }

        public void When(StandardValidationLogModified e)
        {
            var log = _logs[e.Log.LogId];
            log.Set(e.Log);
        }

        public void When(StandardValidationLogRemoved e)
        {
            _logs.Remove(e.LogId);
        }

        #endregion

        #region Methods (helpers)

        private static readonly (string SelfValidationStatus, string ValidationStatus)[] StatusMapping = new[]
        {
            (SelfAssessedStatuses.Expired, ValidationStatuses.Expired),
            (SelfAssessedStatuses.NotCompleted, ValidationStatuses.NotCompleted),
            (SelfAssessedStatuses.NotApplicable, ValidationStatuses.NotApplicable),
            (SelfAssessedStatuses.NeedsTraining, ValidationStatuses.NeedsTraining),
            (SelfAssessedStatuses.SelfAssessed, ValidationStatuses.SelfAssessed)
        };

        private void SetModified(Change e)
        {
            SetValue(StandardValidationField.ModifiedBy, e.OriginUser, false);
            SetValue(StandardValidationField.Modified, e.ChangeTime, false);
        }

        public bool HasLog(Guid logId)
        {
            return _logs.ContainsKey(logId);
        }

        public StandardValidationLog GetLog(Guid logId)
        {
            return _logs.GetOrDefault(logId);
        }

        public IEnumerable<StandardValidationLog> GetLogs()
        {
            return _logs.Values.AsEnumerable();
        }

        private void AddLog(Change e, Guid logId, string status, string comment)
        {
            _logs.Add(logId, new StandardValidationLog
            {
                LogId = logId,
                AuthorUserId = e.OriginUser,
                Posted = e.ChangeTime,
                Status = status,
                Comment = comment
            });
        }

        #endregion
    }
}
