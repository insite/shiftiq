using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Standards.Read;
using InSite.Domain.Standards;

namespace InSite.Persistence
{
    public class QStandardValidationStore : IStandardValidationStore
    {
        public void DeleteAll()
        {
            const string sql = @"
TRUNCATE TABLE [standard].QStandardValidation;
TRUNCATE TABLE [standard].QStandardValidationLog;
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql);
        }

        public void DeleteAll(Guid id)
        {
            const string sql = @"
DELETE [standard].QStandardValidation WHERE StandardValidationIdentifier = @ID;
DELETE [standard].QStandardValidationLog WHERE StandardValidationIdentifier = @ID;
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql, new[]
                {
                    new SqlParameter("ID", id)
                });
        }

        public void Insert(StandardValidationCreated e)
        {
            var state = (StandardValidationState)e.AggregateState;
            var entity = new QStandardValidation
            {
                StandardValidationIdentifier = e.AggregateIdentifier,
                OrganizationIdentifier = state.GetGuidValue(StandardValidationField.OrganizationIdentifier).Value,
                StandardIdentifier = state.GetGuidValue(StandardValidationField.StandardIdentifier).Value,
                UserIdentifier = state.GetGuidValue(StandardValidationField.UserIdentifier).Value,
                ValidationStatus = state.GetTextValue(StandardValidationField.ValidationStatus),
                IsValidated = state.GetBoolValue(StandardValidationField.IsValidated).Value,
            };

            SetTimestamp(entity, e);

            using (var db = new InternalDbContext())
            {
                db.QStandardValidations.Add(entity);
                db.SaveChanges();
            };
        }

        public void Delete(StandardValidationRemoved e)
        {
            using (var db = new InternalDbContext())
            {
                var validation = db.QStandardValidations.FirstOrDefault(x => x.StandardValidationIdentifier == e.AggregateIdentifier);
                if (validation == null)
                    return;

                var logs = db.QStandardValidationLogs
                    .Where(x => x.StandardValidationIdentifier == validation.StandardValidationIdentifier)
                    .ToArray();

                db.QStandardValidationLogs.RemoveRange(logs);
                db.QStandardValidations.Remove(validation);

                db.SaveChanges();
            };
        }

        public void Update(StandardValidationTimestampsModified e) => Update(e, (db, entity) =>
        {

        });

        public void Update(StandardValidationFieldTextModified e) => Update(e, (db, entity) =>
        {
            SetEntityField(entity, (StandardValidationState)e.AggregateState, e.Field);
        });

        public void Update(StandardValidationFieldDateOffsetModified e) => Update(e, (db, entity) =>
        {
            SetEntityField(entity, (StandardValidationState)e.AggregateState, e.Field);
        });

        public void Update(StandardValidationFieldBoolModified e) => Update(e, (db, entity) =>
        {
            SetEntityField(entity, (StandardValidationState)e.AggregateState, e.Field);
        });

        public void Update(StandardValidationFieldGuidModified e) => Update(e, (db, entity) =>
        {
            SetEntityField(entity, (StandardValidationState)e.AggregateState, e.Field);
        });

        public void Update(StandardValidationFieldsModified e) => Update(e, (db, entity) =>
        {
            var state = (StandardValidationState)e.AggregateState;
            foreach (var field in e.Fields.Keys)
                SetEntityField(entity, state, field);
        });

        public void Update(StandardValidationSelfValidated e) => Update(e, (db, entity) =>
        {
            var state = (StandardValidationState)e.AggregateState;

            entity.SelfAssessmentStatus = state.GetTextValue(StandardValidationField.SelfAssessmentStatus);
            entity.SelfAssessmentDate = state.GetDateOffsetValue(StandardValidationField.SelfAssessmentDate);
            entity.Expired = state.GetDateOffsetValue(StandardValidationField.Expired);
            entity.Notified = state.GetDateOffsetValue(StandardValidationField.Notified);
            entity.ValidationDate = state.GetDateOffsetValue(StandardValidationField.ValidationDate);
            entity.IsValidated = state.GetBoolValue(StandardValidationField.IsValidated) ?? false;
            entity.ValidatorUserIdentifier = state.GetGuidValue(StandardValidationField.ValidatorUserIdentifier);
            entity.ValidationComment = state.GetTextValue(StandardValidationField.ValidationComment);
            entity.ValidationStatus = state.GetTextValue(StandardValidationField.ValidationStatus);

            var log = state.GetLog(e.LogId);
            if (log != null)
            {
                var logEntity = CreateLog(e, log);

                db.QStandardValidationLogs.Add(logEntity);
            }
        });

        public void Update(StandardValidationStatusModified e) => Update(e, (db, entity) =>
        {
            var state = (StandardValidationState)e.AggregateState;

            entity.SelfAssessmentStatus = state.GetTextValue(StandardValidationField.SelfAssessmentStatus);
            entity.SelfAssessmentDate = state.GetDateOffsetValue(StandardValidationField.SelfAssessmentDate);
            entity.Expired = state.GetDateOffsetValue(StandardValidationField.Expired);
            entity.Notified = state.GetDateOffsetValue(StandardValidationField.Notified);
            entity.ValidationDate = state.GetDateOffsetValue(StandardValidationField.ValidationDate);
            entity.IsValidated = state.GetBoolValue(StandardValidationField.IsValidated) ?? false;
            entity.ValidatorUserIdentifier = state.GetGuidValue(StandardValidationField.ValidatorUserIdentifier);
            entity.ValidationComment = state.GetTextValue(StandardValidationField.ValidationComment);
            entity.ValidationStatus = state.GetTextValue(StandardValidationField.ValidationStatus);

            var log = state.GetLog(e.LogId);
            if (log != null)
            {
                var logEntity = CreateLog(e, log);

                db.QStandardValidationLogs.Add(logEntity);
            }
        });

        public void Update(StandardValidationSubmittedForValidation e) => Update(e, (db, entity) =>
        {
            var state = (StandardValidationState)e.AggregateState;

            entity.ValidationStatus = state.GetTextValue(StandardValidationField.ValidationStatus);

            var log = state.GetLog(e.LogId);
            var logEntity = CreateLog(e, log);

            db.QStandardValidationLogs.Add(logEntity);
        });

        public void Update(StandardValidationValidated e) => Update(e, (db, entity) =>
        {
            var state = (StandardValidationState)e.AggregateState;

            entity.ValidationStatus = state.GetTextValue(StandardValidationField.ValidationStatus);
            entity.ValidationDate = state.GetDateOffsetValue(StandardValidationField.ValidationDate);
            entity.ValidationComment = state.GetTextValue(StandardValidationField.ValidationComment);
            entity.IsValidated = state.GetBoolValue(StandardValidationField.IsValidated) ?? false;
            entity.ValidatorUserIdentifier = state.GetGuidValue(StandardValidationField.ValidatorUserIdentifier);
            entity.SelfAssessmentStatus = state.GetTextValue(StandardValidationField.SelfAssessmentStatus);

            var logEntity = CreateLog(e, state.GetLog(e.LogId));

            db.QStandardValidationLogs.Add(logEntity);
        });

        public void Update(StandardValidationExpired e) => Update(e, (db, entity) =>
        {
            var state = (StandardValidationState)e.AggregateState;

            entity.Expired = state.GetDateOffsetValue(StandardValidationField.Expired);
            entity.Notified = state.GetDateOffsetValue(StandardValidationField.Notified);
            entity.ValidationStatus = state.GetTextValue(StandardValidationField.ValidationStatus);
            entity.SelfAssessmentStatus = state.GetTextValue(StandardValidationField.SelfAssessmentStatus);
            entity.SelfAssessmentDate = state.GetDateOffsetValue(StandardValidationField.SelfAssessmentDate);

            var logEntity = CreateLog(e, state.GetLog(e.LogId));

            db.QStandardValidationLogs.Add(logEntity);
        });

        public void Update(StandardValidationNotified e) => Update(e, (db, entity) =>
        {
            var state = (StandardValidationState)e.AggregateState;
            entity.Notified = state.GetDateOffsetValue(StandardValidationField.Notified);
        });

        public void Update(StandardValidationLogAdded e)
        {
            var entities = CreateLogs(e, e.Logs);

            using (var db = new InternalDbContext())
            {
                db.QStandardValidationLogs.AddRange(entities);
                db.SaveChanges();
            };
        }

        private static QStandardValidationLog CreateLog(Change e, StandardValidationLog log)
        {
            return CreateLogs(e, new[] { log })[0];
        }

        private static QStandardValidationLog[] CreateLogs(Change e, IEnumerable<StandardValidationLog> logs)
        {
            var state = (StandardValidationState)e.AggregateState;
            var organizationId = state.GetGuidValue(StandardValidationField.OrganizationIdentifier).Value;
            var standardId = state.GetGuidValue(StandardValidationField.StandardIdentifier).Value;
            var userId = state.GetGuidValue(StandardValidationField.UserIdentifier).Value;

            return logs.Select(x => new QStandardValidationLog
            {
                LogIdentifier = x.LogId,
                OrganizationIdentifier = organizationId,
                StandardValidationIdentifier = e.AggregateIdentifier,
                StandardIdentifier = standardId,
                UserIdentifier = userId,
                AuthorUserIdentifier = x.AuthorUserId ?? e.OriginUser,
                LogComment = x.Comment,
                LogPosted = x.Posted ?? e.ChangeTime,
                LogStatus = x.Status
            }).ToArray();
        }

        public void Update(StandardValidationLogModified e)
        {
            var log = e.Log;

            using (var db = new InternalDbContext())
            {
                var entity = db.QStandardValidationLogs.FirstOrDefault(x => x.LogIdentifier == log.LogId);
                entity.AuthorUserIdentifier = log.AuthorUserId;
                entity.LogPosted = log.Posted.Value;
                entity.LogStatus = log.Status;
                entity.LogComment = log.Comment;
                db.SaveChanges();
            };
        }

        public void Update(StandardValidationLogRemoved e)
        {
            using (var db = new InternalDbContext())
            {
                var log = db.QStandardValidationLogs.FirstOrDefault(x => x.StandardValidationIdentifier == e.AggregateIdentifier && x.LogIdentifier == e.LogId);
                if (log == null)
                    return;

                db.QStandardValidationLogs.Remove(log);
                db.SaveChanges();
            };
        }

        private void Update(Change e, Action<InternalDbContext, QStandardValidation> action)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QStandardValidations.FirstOrDefault(x => x.StandardValidationIdentifier == e.AggregateIdentifier);
                if (entity == null)
                    return;

                action(db, entity);

                SetTimestamp(entity, e);

                db.SaveChanges();
            };
        }

        private static void SetTimestamp(QStandardValidation entity, Change e)
        {
            var state = (StandardValidationState)e.AggregateState;

            entity.Created = state.GetDateOffsetValue(StandardValidationField.Created).Value;
            entity.CreatedBy = state.GetGuidValue(StandardValidationField.CreatedBy).Value;
            entity.Modified = state.GetDateOffsetValue(StandardValidationField.Modified).Value;
            entity.ModifiedBy = state.GetGuidValue(StandardValidationField.ModifiedBy).Value;
        }

        private static void SetEntityField(QStandardValidation entity, StandardValidationState state, StandardValidationField field)
        {
            switch (field)
            {
                // string

                case StandardValidationField.SelfAssessmentStatus:
                    entity.SelfAssessmentStatus = state.GetTextValue(field);
                    break;
                case StandardValidationField.ValidationComment:
                    entity.ValidationComment = state.GetTextValue(field);
                    break;
                case StandardValidationField.ValidationStatus:
                    entity.ValidationStatus = state.GetTextValue(field);
                    break;

                // DateTimeOffset

                case StandardValidationField.Created:
                    entity.Created = state.GetDateOffsetValue(field).Value;
                    break;
                case StandardValidationField.Expired:
                    entity.Expired = state.GetDateOffsetValue(field);
                    break;
                case StandardValidationField.Modified:
                    entity.Modified = state.GetDateOffsetValue(field).Value;
                    break;
                case StandardValidationField.Notified:
                    entity.Notified = state.GetDateOffsetValue(field);
                    break;
                case StandardValidationField.SelfAssessmentDate:
                    entity.SelfAssessmentDate = state.GetDateOffsetValue(field);
                    break;
                case StandardValidationField.ValidationDate:
                    entity.ValidationDate = state.GetDateOffsetValue(field);
                    break;

                // bool

                case StandardValidationField.IsValidated:
                    entity.IsValidated = state.GetBoolValue(field).Value;
                    break;

                // Guid

                case StandardValidationField.StandardIdentifier:
                    entity.StandardIdentifier = state.GetGuidValue(field).Value;
                    break;
                case StandardValidationField.UserIdentifier:
                    entity.UserIdentifier = state.GetGuidValue(field).Value;
                    break;
                case StandardValidationField.ValidatorUserIdentifier:
                    entity.ValidatorUserIdentifier = state.GetGuidValue(field);
                    break;
                case StandardValidationField.OrganizationIdentifier:
                    entity.OrganizationIdentifier = state.GetGuidValue(field);
                    break;
                case StandardValidationField.CreatedBy:
                    entity.CreatedBy = state.GetGuidValue(field).Value;
                    break;
                case StandardValidationField.ModifiedBy:
                    entity.ModifiedBy = state.GetGuidValue(field).Value;
                    break;

                default:
                    throw new ArgumentException($"Unsupported standard field: {field}");
            }
        }
    }
}
