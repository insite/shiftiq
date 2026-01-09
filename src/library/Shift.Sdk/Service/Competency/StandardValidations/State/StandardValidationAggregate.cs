using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Domain.Standards
{
    public class StandardValidationAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new StandardValidationState();

        public StandardValidationState Data => (StandardValidationState)State;

        public void CreateStandardValidation(Guid standardId, Guid userId)
        {
            if (AggregateIdentifier == default)
                throw new AggregateException("AggregateIdentifier is empty");

            if (standardId == default)
                throw new AggregateException("StandardIdentifier is empty");

            if (userId == default)
                throw new AggregateException("UserIdentifier is empty");

            Apply(new StandardValidationCreated(standardId, userId));
        }

        public void RemoveStandardValidation()
        {
            if (Data.IsExist)
                Apply(new StandardValidationRemoved());
        }

        public void RemoveStandardValidationLog(Guid logId)
        {
            if (Data.HasLog(logId))
                Apply(new StandardValidationLogRemoved(logId));
        }

        public void ModifyStandardValidationTimestamps(DateTimeOffset created, Guid createdBy, DateTimeOffset modified, Guid modifiedBy)
        {
            if (created == default)
                throw new AggregateException("Created is empty");

            if (createdBy == default)
                throw new AggregateException("CreatedBy is empty");

            if (modified == default)
                throw new AggregateException("Modified is empty");

            if (modifiedBy == default)
                throw new AggregateException("ModifiedBy is empty");

            var isChanged = Data.GetDateOffsetValue(StandardValidationField.Created) != created
                || Data.GetGuidValue(StandardValidationField.CreatedBy) != createdBy
                || Data.GetDateOffsetValue(StandardValidationField.Modified) != modified
                || Data.GetGuidValue(StandardValidationField.ModifiedBy) != modifiedBy;

            if (!isChanged)
                return;

            Apply(new StandardValidationTimestampsModified(created, createdBy, modified, modifiedBy));
        }

        public void ModifyStandardValidationFieldText(StandardValidationField field, string value)
        {
            if (value.IsEmpty())
                value = null;

            StandardValidationFieldList.Validate(field, value, true);

            if (Data.GetTextValue(field) == value)
                return;

            Apply(new StandardValidationFieldTextModified(field, value));
        }

        public void ModifyStandardValidationFieldDateOffset(StandardValidationField field, DateTimeOffset? value)
        {
            StandardValidationFieldList.Validate(field, value, true);

            if (Data.GetDateOffsetValue(field) == value)
                return;

            Apply(new StandardValidationFieldDateOffsetModified(field, value));
        }

        public void ModifyStandardValidationFieldBool(StandardValidationField field, bool? value)
        {
            StandardValidationFieldList.Validate(field, value, true);

            if (Data.GetBoolValue(field) == value)
                return;

            Apply(new StandardValidationFieldBoolModified(field, value));
        }

        public void ModifyStandardValidationFieldGuid(StandardValidationField field, Guid? value)
        {
            StandardValidationFieldList.Validate(field, value, true);

            if (Data.GetGuidValue(field) == value)
                return;

            Apply(new StandardValidationFieldGuidModified(field, value));
        }

        public void ModifyStandardValidationFields(IDictionary<StandardValidationField, object> values)
        {
            var changeValues = new Dictionary<StandardValidationField, object>();

            foreach (var kv in values)
            {
                var field = kv.Key;
                var info = StandardValidationFieldList.GetField(field);
                var type = info.FieldType;

                if (type == StateFieldType.Text)
                {
                    var value = (string)kv.Value;
                    if (value.IsEmpty())
                        value = null;

                    StandardValidationFieldList.Validate(info, field, value, true);

                    if (!StringHelper.EqualsCaseSensitive(Data.GetTextValue(field), value))
                        changeValues[field] = value;
                }
                else if (type == StateFieldType.DateOffset)
                {
                    var value = (DateTimeOffset?)kv.Value;

                    StandardValidationFieldList.Validate(info, field, value, true);

                    if (Data.GetDateOffsetValue(field) != value)
                        changeValues[field] = value;
                }
                else if (type == StateFieldType.Bool)
                {
                    var value = (bool?)kv.Value;

                    StandardValidationFieldList.Validate(info, field, value, true);

                    if (Data.GetBoolValue(field) != value)
                        changeValues[field] = value;
                }
                else if (type == StateFieldType.Guid)
                {
                    var value = (Guid?)kv.Value;

                    StandardValidationFieldList.Validate(info, field, value, true);

                    if (Data.GetGuidValue(field) != value)
                        changeValues[field] = value;
                }
                else
                    throw ApplicationError.Create("Unexpected field type: " + type.GetName());
            }

            if (changeValues.IsEmpty())
                return;

            Apply(new StandardValidationFieldsModified(changeValues));
        }

        public void SelfValidateStandardValidation(Guid logId, string status)
        {
            if (logId == default)
                throw new AggregateException("LogIdentifier is empty");

            if (Data.HasLog(logId))
                throw new AggregateException("LogIdentifier is already used: " + logId);

            if (status.IsEmpty())
                throw new AggregateException("Status is empty");

            // No action should be taken if the self-assessment status is unchanged.

            if (Data.GetTextValue(StandardValidationField.SelfAssessmentStatus) != status)
                Apply(new StandardValidationSelfValidated(logId, status));
        }

        public void SubmitForValidationStandardValidation(Guid logId)
        {
            if (logId == default)
                throw new AggregateException("LogIdentifier is empty");

            if (Data.HasLog(logId))
                throw new AggregateException("LogIdentifier is already used: " + logId);

            if (Data.GetDateOffsetValue(StandardValidationField.ValidationDate).HasValue)
                return;

            var validationStatus = Data.GetTextValue(StandardValidationField.ValidationStatus);
            if (validationStatus != ValidationStatuses.SelfAssessed && validationStatus != ValidationStatuses.NotApplicable)
                return;

            Apply(new StandardValidationSubmittedForValidation(logId));
        }

        public void ValidateStandardValidation(Guid logId, bool isValidated, string status, string comment)
        {
            if (logId == default)
                throw new AggregateException("LogIdentifier is empty");

            if (Data.HasLog(logId))
                throw new AggregateException("LogIdentifier is already used: " + logId);

            if (status.IsEmpty())
                throw new AggregateException("Status is empty");

            if (comment.IsEmpty())
                throw new AggregateException("Comment is empty");

            Apply(new StandardValidationValidated(logId, isValidated, status, comment));
        }

        public void ExpireStandardValidation(Guid logId, string comment)
        {
            if (logId == default)
                throw new AggregateException("LogIdentifier is empty");

            if (Data.HasLog(logId))
                throw new AggregateException("LogIdentifier is already used: " + logId);

            if (comment.IsEmpty())
                throw new AggregateException("Comment is empty");

            var isChanged = Data.GetDateOffsetValue(StandardValidationField.Expired) == null
                || Data.GetTextValue(StandardValidationField.SelfAssessmentStatus) != SelfAssessedStatuses.Expired
                || Data.GetTextValue(StandardValidationField.ValidationStatus) != ValidationStatuses.Expired;

            if (isChanged)
                Apply(new StandardValidationExpired(logId, comment));
        }

        public void NotifyStandardValidation(DateTimeOffset? date)
        {
            if (Data.GetDateOffsetValue(StandardValidationField.Notified) != date)
                Apply(new StandardValidationNotified(date));
        }

        public void AddStandardValidationLog(StandardValidationLog[] logs)
        {
            if (logs.IsEmpty())
                return;

            var changeData = new List<StandardValidationLog>(logs.Length);

            foreach (var log in logs)
            {
                if (log.LogId == default)
                    throw new AggregateException("LogIdentifier is empty");

                if (log.AuthorUserId.HasValue && log.AuthorUserId.Value == default)
                    throw new AggregateException("AuthorUserIdentifier is empty");

                if (log.Status != null && log.Status.Length == 0)
                    log.Status = null;

                if (log.Comment != null && log.Comment.Length == 0)
                    log.Comment = null;

                if (!Data.HasLog(log.LogId))
                    changeData.Add(log.Clone());
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardValidationLogAdded(changeData.ToArray()));
        }

        public void ModifyStandardValidationLog(StandardValidationLog log)
        {
            if (log.LogId == default)
                throw new AggregateException("LogIdentifier is empty");

            if (!log.AuthorUserId.HasValue || log.AuthorUserId.Value == default)
                throw new AggregateException("AuthorUserIdentifier is empty");

            if (!log.Posted.HasValue || log.Posted.Value == default)
                throw new AggregateException("Posted is empty");

            var currentLog = Data.GetLog(log.LogId);
            if (currentLog == null)
                return;

            if (log.Status != null && log.Status.Length == 0)
                log.Status = null;

            if (log.Comment != null && log.Comment.Length == 0)
                log.Comment = null;

            var isChanged = currentLog.AuthorUserId != log.AuthorUserId
                || currentLog.Posted != log.Posted
                || currentLog.Status != log.Status
                || currentLog.Comment != log.Comment;

            if (isChanged)
                Apply(new StandardValidationLogModified(log.Clone()));
        }

        public void ModifyStandardValidationStatus(Guid logId, bool isValidated, string selfAssessmentStatus, string validationStatus, string validationComment)
        {
            if (logId == default)
                throw new AggregateException("LogIdentifier is empty");

            if (Data.HasLog(logId))
                throw new AggregateException("LogIdentifier is already used: " + logId);

            if (selfAssessmentStatus.IsEmpty())
                selfAssessmentStatus = null;

            if (validationStatus.IsEmpty())
                throw new AggregateException("ValidationStatus is empty");

            if (validationComment.IsEmpty())
                validationComment = null;

            // No action should be taken if the status is unchanged.

            var isChanged = Data.GetTextValue(StandardValidationField.SelfAssessmentStatus) != selfAssessmentStatus
                || Data.GetTextValue(StandardValidationField.ValidationStatus) != validationStatus
                || Data.GetTextValue(StandardValidationField.ValidationComment) != validationComment;

            if (isChanged)
                Apply(new StandardValidationStatusModified(logId, isValidated, selfAssessmentStatus, validationStatus, validationComment));
        }
    }
}
