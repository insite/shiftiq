using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Domain.Courses;

using Shift.Common;
using Shift.Constant;

using DomainCourseState = InSite.Domain.Courses.CourseState;

namespace InSite.Persistence
{
    public partial class QCourseStore : ICourseStore
    {
        private readonly ConcurrentDictionary<Guid, TransactionState> _transactions = new ConcurrentDictionary<Guid, TransactionState>();
        private readonly IContentStore _contentStore;

        public QCourseStore(IContentStore contentStore)
        {
            _contentStore = contentStore;
        }

        #region Transactions

        public Guid StartTransaction(Guid courseId)
        {
            var transactionId = Guid.NewGuid();

            if (!_transactions.TryAdd(transactionId, new TransactionState(courseId)))
                throw new ArgumentException($"TransactionId is already used: {transactionId}");

            return transactionId;
        }

        public void CancelTransaction(Guid transactionId)
        {
            _transactions.TryRemove(transactionId, out _);
        }

        public void CommitTransaction(Guid transactionId)
        {
            if (!_transactions.TryGetValue(transactionId, out var transaction))
                throw new ArgumentException($"Transaction does not exist: {transactionId}");

            using (var db = CreateContext())
            {
                RemoveActivities(db, transaction.Activities.Values);
                RemoveModules(db, transaction.Modules.Values);
                RemoveUnits(db, transaction.Units.Values);

                if (transaction.Course != null)
                    SaveEntity(db, transaction.Course);

                SaveEntities(db, transaction.Enrollments.Values);
                SaveEntities(db, transaction.Units.Values);
                SaveEntities(db, transaction.Modules.Values);
                SaveEntities(db, transaction.Activities.Values);
                SaveEntities(db, transaction.Prerequisites.Values);

                SaveCompetencies(db, transaction);

                db.SaveChanges();
            }

            if (transaction.Course != null)
                SaveContent(transaction.Course);

            SaveContents(transaction.Units.Values);
            SaveContents(transaction.Modules.Values);
            SaveContents(transaction.Activities.Values);

            _transactions.TryRemove(transactionId, out _);
        }

        private void SaveEntity(InternalDbContext db, IEntityState entityState)
        {
            if (entityState.Operation == EntityOperation.Ignore)
                return;

            if (entityState.EntityObject == null)
                throw new ArgumentNullException("entityState.EntityObject");

            switch (entityState.Operation)
            {
                case EntityOperation.Insert:
                    db.Entry(entityState.EntityObject).State = EntityState.Added;
                    break;
                case EntityOperation.Modify:
                    db.Entry(entityState.EntityObject).State = EntityState.Modified;
                    break;
                case EntityOperation.Remove:
                    if (entityState.EntityObject is QCourseEnrollment || entityState.EntityObject is QCoursePrerequisite)
                        db.Entry(entityState.EntityObject).State = EntityState.Deleted;
                    break;
                default:
                    throw new ArgumentException($"Unsupported operation: {entityState.Operation}");
            }
        }

        private void SaveEntities(InternalDbContext db, IEnumerable<IEntityState> entityStates)
        {
            foreach (var entityState in entityStates)
                SaveEntity(db, entityState);
        }

        private void SaveContent(IEntityState entityState)
        {
            if (entityState.Content == null)
                return;

            switch (entityState.Operation)
            {
                case EntityOperation.Insert:
                case EntityOperation.Modify:
                    _contentStore.SaveContainer(entityState.OrganizationId, entityState.ContainerType, entityState.ContainerId, entityState.Content);
                    break;
                case EntityOperation.Remove:
                    _contentStore.DeleteContainer(entityState.ContainerId);
                    break;
                default:
                    throw new ArgumentException($"Unsupported operation: {entityState.Operation}");
            }
        }

        private void SaveContents(IEnumerable<IEntityState> entityStates)
        {
            foreach (var entityState in entityStates)
                SaveContent(entityState);
        }

        private void SaveCompetencies(InternalDbContext db, TransactionState transaction)
        {
            foreach (var pair in transaction.Competencies)
            {
                var activityId = pair.Key;
                var competencyChanges = pair.Value;

                var all = db.QActivityCompetencies.Where(x => x.ActivityIdentifier == activityId).ToList();
                var remove = all.Where(x => competencyChanges.Remove.Contains(x.CompetencyStandardIdentifier)).ToList();
                var insert = competencyChanges.Insert.Where(x => !all.Any(y => y.CompetencyStandardIdentifier == x.CompetencyStandardIdentifier)).ToList();

                var modify = new List<QActivityCompetency>();
                foreach (var existing in all)
                {
                    foreach (var c in competencyChanges.Insert)
                    {
                        if (existing.CompetencyStandardIdentifier != c.CompetencyStandardIdentifier)
                            continue;

                        existing.CompetencyCode = c.CompetencyCode;
                        existing.RelationshipType = c.RelationshipType;

                        modify.Add(existing);
                    }
                }

                foreach (var entity in modify)
                    db.Entry(entity).State = EntityState.Modified;

                db.QActivityCompetencies.RemoveRange(remove);
                db.QActivityCompetencies.AddRange(insert);
            }
        }

        private void RemoveUnits(InternalDbContext db, IEnumerable<EntityState<QUnit>> unitStates)
        {
            foreach (var unitState in unitStates)
            {
                if (unitState.Operation == EntityOperation.Remove)
                    db.Database.ExecuteSqlCommand("courses.RemoveUnit @UnitId", new SqlParameter("UnitId", unitState.Entity.UnitIdentifier));
            }
        }

        private void RemoveModules(InternalDbContext db, IEnumerable<EntityState<QModule>> moduleStates)
        {
            foreach (var moduleState in moduleStates)
            {
                if (moduleState.Operation == EntityOperation.Remove)
                    db.Database.ExecuteSqlCommand("courses.RemoveModule @ModuleId", new SqlParameter("ModuleId", moduleState.Entity.ModuleIdentifier));
            }
        }

        private void RemoveActivities(InternalDbContext db, IEnumerable<EntityState<QActivity>> activityStates)
        {
            foreach (var activityState in activityStates)
            {
                if (activityState.Operation == EntityOperation.Remove)
                    db.Database.ExecuteSqlCommand("courses.RemoveActivity @ActivityId", new SqlParameter("ActivityId", activityState.Entity.ActivityIdentifier));
            }
        }

        #endregion

        #region Courses

        public void InsertCourse(CourseCreated e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var transaction = _transactions[transactionId];
            if (transaction.Course != null)
                throw new ArgumentException("Invalid change: CourseCreated");

            var course = new QCourse
            {
                CourseIdentifier = e.AggregateIdentifier,
                OrganizationIdentifier = e.OrganizationId,
                CourseAsset = e.CourseAsset,
                CourseName = e.CourseName,
                CourseSequence = DomainCourseState.Defaults.CourseSequence,
                CourseIsHidden = DomainCourseState.Defaults.IsHidden,
                IsMultipleUnitsEnabled = DomainCourseState.Defaults.IsMultipleUnitsEnabled,
                IsProgressReportEnabled = DomainCourseState.Defaults.IsProgressReportEnabled,
                AllowDiscussion = DomainCourseState.Defaults.AllowDiscussion,
                Created = e.ChangeTime,
                CreatedBy = e.OriginUser
            };

            ModifyTimestamp(course, null, null, null, course.Created, course.CreatedBy);

            transaction.Course = new EntityState<QCourse>(EntityOperation.Insert, course, e.OrganizationId, e.AggregateIdentifier, ContentContainerType.Course, e.CourseContent);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void ModifyCourse(Change e, ContentContainer content, Action<QCourse> action)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var courseState = GetTransaction(transactionId, e.AggregateIdentifier).Course;
            if (courseState.Entity != null)
            {
                if (content != null)
                    courseState.Content = content;

                ModifyTimestamp(courseState.Entity, null, null, null, e.ChangeTime, e.OriginUser);

                action?.Invoke(courseState.Entity);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void RemoveCourse(CourseDeleted e)
        {
            using (var db = CreateContext())
            {
                db.Database.ExecuteSqlCommand("courses.RemoveCourse @CourseId", new SqlParameter("CourseId", e.AggregateIdentifier));
            }
        }

        private TransactionState GetTransaction(Guid transactionId, Guid courseId)
        {
            var transaction = _transactions[transactionId];

            if (transaction.CourseId != courseId)
                throw new ArgumentException($"Transaction ${transactionId} stores course ${transaction.CourseId}, but was requested ${courseId}");

            if (transaction.Course == null)
            {
                using (var db = CreateContext())
                {
                    var course = db.QCourses.Where(x => x.CourseIdentifier == courseId).FirstOrDefault();
                    transaction.Course = new EntityState<QCourse>(
                        EntityOperation.Modify,
                        course,
                        course?.OrganizationIdentifier ?? Guid.Empty,
                        courseId,
                        ContentContainerType.Course,
                        null
                    );
                }
            }
            else if (transaction.Course.Operation == EntityOperation.Remove)
                throw new ArgumentException($"Cannot modify deleted entity");

            return transaction;
        }

        #endregion

        #region Enrollments

        public void InsertEnrollment(CourseEnrollmentAdded e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var transaction = GetTransaction(transactionId, e.AggregateIdentifier);
            if (transaction.Course.Entity == null)
                throw new ArgumentException($"QCourse does not exist: {e.AggregateIdentifier}");

            if (transaction.Enrollments.ContainsKey(e.CourseEnrollmentId))
                throw new ArgumentException("Invalid change: CourseEnrolled");

            var enrollment = new QCourseEnrollment
            {
                CourseEnrollmentIdentifier = e.CourseEnrollmentId,
                CourseIdentifier = e.AggregateIdentifier,
                OrganizationIdentifier = transaction.Course.Entity.OrganizationIdentifier,
                LearnerUserIdentifier = e.LearnerUserId,
                CourseStarted = e.CourseStarted
            };

            transaction.Enrollments.Add(e.CourseEnrollmentId, new EntityState<QCourseEnrollment>(EntityOperation.Insert, enrollment));

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void ModifyEnrollment(Change e, Guid enrollmentId, Action<QCourseEnrollment> action)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (_, enrollment) = GetEnrollment(transactionId, e.AggregateIdentifier, enrollmentId, EntityOperation.Modify);
            if (enrollment != null)
                action(enrollment);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void RemoveEnrollment(CourseEnrollmentRemoved e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            GetEnrollment(transactionId, e.AggregateIdentifier, e.CourseEnrollmentId, EntityOperation.Remove);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        private (TransactionState, QCourseEnrollment) GetEnrollment(Guid transactionId, Guid courseId, Guid enrollmentId, EntityOperation operation)
        {
            var transaction = GetTransaction(transactionId, courseId);
            if (transaction.Course.Entity == null)
                return (transaction, null);

            if (!transaction.Enrollments.TryGetValue(enrollmentId, out var enrollmentState))
            {
                using (var db = CreateContext())
                {
                    var enrollment = db.QCourseEnrollments.Where(x => x.CourseEnrollmentIdentifier == enrollmentId).FirstOrDefault();
                    transaction.Enrollments.Add(enrollmentId, enrollmentState = new EntityState<QCourseEnrollment>(operation, enrollment));
                }
            }
            else if (enrollmentState.Operation != operation && (enrollmentState.Operation != EntityOperation.Insert || operation != EntityOperation.Modify))
                throw new ArgumentException($"Invalid operation: {operation}");

            return (transaction, enrollmentState.Entity);
        }

        #endregion

        #region Prerequisites

        public void InsertPrerequisite(Change e, Prerequisite p, Guid objectId, PrerequisiteObjectType objectType)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var transaction = GetTransaction(transactionId, e.AggregateIdentifier);
            if (transaction.Course.Entity == null)
                throw new ArgumentException($"QCourse does not exist: {e.AggregateIdentifier}");

            if (transaction.Prerequisites.ContainsKey(p.Identifier))
                throw new ArgumentException($"Invalid change: {e.ChangeType}");

            if (!ModifyPrerequisiteObject(e, transactionId, objectId, objectType))
                return;

            var prerequisite = new QCoursePrerequisite
            {
                CoursePrerequisiteIdentifier = p.Identifier,
                CourseIdentifier = e.AggregateIdentifier,
                OrganizationIdentifier = transaction.Course.Entity.OrganizationIdentifier,
                ObjectIdentifier = objectId,
                ObjectType = objectType.ToString(),
                TriggerChange = p.TriggerChange.ToString(),
                TriggerConditionScoreFrom = p.TriggerConditionScoreFrom,
                TriggerConditionScoreThru = p.TriggerConditionScoreThru,
                TriggerIdentifier = p.TriggerIdentifier,
                TriggerType = p.TriggerType.ToString()
            };

            transaction.Prerequisites.Add(p.Identifier, new EntityState<QCoursePrerequisite>(EntityOperation.Insert, prerequisite));

            ModifyTimestamp(transaction.Course.Entity, null, null, null, e.ChangeTime, e.OriginUser);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void RemovePrerequisite(Change e, Guid prerequisiteId)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, prerequisite) = GetPrerequisite(transactionId, e.AggregateIdentifier, prerequisiteId);
            if (transaction.Course.Entity == null)
                throw new ArgumentException($"QCourse does not exist: {e.AggregateIdentifier}");

            if (prerequisite != null)
            {
                var objectType = ParseObjectType(prerequisite.ObjectType);
                if (objectType.HasValue)
                    ModifyPrerequisiteObject(e, transactionId, prerequisite.ObjectIdentifier, objectType.Value);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        private bool ModifyPrerequisiteObject(Change e, Guid transactionId, Guid objectId, PrerequisiteObjectType objectType)
        {
            if (objectType == PrerequisiteObjectType.Unit)
            {
                var (transaction, unitState) = GetUnitState(transactionId, e.AggregateIdentifier, objectId, EntityOperation.Modify);
                if (unitState.Entity == null)
                    return false;

                ModifyTimestamp(transaction.Course.Entity, unitState.Entity, null, null, e.ChangeTime, e.OriginUser);
            }
            else if (objectType == PrerequisiteObjectType.Module)
            {
                var (transaction, unit, moduleState) = GetModuleState(transactionId, e.AggregateIdentifier, objectId, EntityOperation.Modify);
                if (moduleState.Entity == null)
                    return false;

                ModifyTimestamp(transaction.Course.Entity, unit, moduleState.Entity, null, e.ChangeTime, e.OriginUser);
            }
            else if (objectType == PrerequisiteObjectType.Activity)
            {
                var (transaction, unit, module, activityState) = GetActivityState(transactionId, e.AggregateIdentifier, objectId, EntityOperation.Modify);
                if (activityState.Entity == null)
                    return false;

                ModifyTimestamp(transaction.Course.Entity, unit, module, activityState.Entity, e.ChangeTime, e.OriginUser);
            }
            else
                throw new ArgumentException($"Unsupported object type: {objectType}");

            return true;
        }

        private (TransactionState, QCoursePrerequisite) GetPrerequisite(Guid transactionId, Guid courseId, Guid prerequisiteId)
        {
            var transaction = GetTransaction(transactionId, courseId);
            if (transaction.Course.Entity == null)
                return (transaction, null);

            if (!transaction.Prerequisites.TryGetValue(prerequisiteId, out var prerequisiteState))
            {
                using (var db = CreateContext())
                {
                    var prerequisite = db.QCoursePrerequisites.Where(x => x.CoursePrerequisiteIdentifier == prerequisiteId).FirstOrDefault();
                    transaction.Prerequisites.Add(prerequisiteId, prerequisiteState = new EntityState<QCoursePrerequisite>(EntityOperation.Remove, prerequisite));
                }
            }
            else
                throw new ArgumentException($"Invalid operation: Remove");

            return (transaction, prerequisiteState.Entity);
        }

        private PrerequisiteObjectType? ParseObjectType(string objectType)
        {
            if (string.Equals(objectType, "Unit", StringComparison.OrdinalIgnoreCase))
                return PrerequisiteObjectType.Unit;

            if (string.Equals(objectType, "Module", StringComparison.OrdinalIgnoreCase))
                return PrerequisiteObjectType.Module;

            if (string.Equals(objectType, "Activity", StringComparison.OrdinalIgnoreCase))
                return PrerequisiteObjectType.Activity;

            return null;
        }

        #endregion

        #region Competencies

        public void InsertCompetencies(CourseActivityCompetenciesAdded e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unit, module, activity, competencyChanges) = GetCompetencyChanges(transactionId, e.AggregateIdentifier, e.ActivityId);
            if (activity == null)
                return;

            foreach (var c in e.Competencies)
            {
                var existing = competencyChanges.Insert.Find(x => x.CompetencyStandardIdentifier == c.CompetencyStandardIdentifier);
                if (existing != null)
                {
                    existing.CompetencyCode = c.CompetencyCode;
                    existing.RelationshipType = c.RelationshipType;
                    continue;
                }

                competencyChanges.Remove.Remove(c.CompetencyStandardIdentifier);

                competencyChanges.Insert.Add(new QActivityCompetency
                {
                    ActivityIdentifier = e.ActivityId,
                    CompetencyStandardIdentifier = c.CompetencyStandardIdentifier,
                    OrganizationIdentifier = transaction.Course.Entity.OrganizationIdentifier,
                    CompetencyCode = c.CompetencyCode,
                    RelationshipType = c.RelationshipType
                });
            }

            ModifyTimestamp(transaction.Course.Entity, unit, module, activity, e.ChangeTime, e.OriginUser);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void RemoveCompetencies(CourseActivityCompetenciesRemoved e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unit, module, activity, competencyChanges) = GetCompetencyChanges(transactionId, e.AggregateIdentifier, e.ActivityId);
            if (activity == null)
                return;

            foreach (var id in e.CompetencyIds)
            {
                var insertIndex = competencyChanges.Insert.FindIndex(x => x.CompetencyStandardIdentifier == id);
                if (insertIndex >= 0)
                    competencyChanges.Insert.RemoveAt(insertIndex);

                competencyChanges.Remove.Add(id);
            }

            ModifyTimestamp(transaction.Course.Entity, unit, module, activity, e.ChangeTime, e.OriginUser);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        private (TransactionState, QUnit, QModule, QActivity, TransactionState.CompetencyChanges) GetCompetencyChanges(Guid transactionId, Guid courseId, Guid activityId)
        {
            var (transaction, unit, module, activityState) = GetActivityState(transactionId, courseId, activityId, EntityOperation.Modify);
            if (transaction.Course.Entity == null || activityState.Entity == null)
                return (transaction, null, null, null, null);

            if (!transaction.Competencies.TryGetValue(activityId, out var competencyChanges))
            {
                transaction.Competencies.Add(activityId, competencyChanges = new TransactionState.CompetencyChanges());
            }

            return (transaction, unit, module, activityState.Entity, competencyChanges);
        }

        #endregion

        #region Units

        public void InsertUnit(CourseUnitAdded e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var transaction = GetTransaction(transactionId, e.AggregateIdentifier);
            if (transaction.Course.Entity == null)
                throw new ArgumentException($"QCourse does not exist: {e.AggregateIdentifier}");

            if (transaction.Units.ContainsKey(e.UnitId))
                throw new ArgumentException("Invalid change: CourseUnitAdded");

            var unitAggregateState = ((DomainCourseState)e.AggregateState).GetUnit(e.UnitId);
            if (unitAggregateState == null) // Make sure the Unit was not removed from the aggregate when we use multiple changes
            {
                transaction.Units.Add(e.UnitId, new EntityState<QUnit>(EntityOperation.Ignore, null));
                return;
            }

            var unit = new QUnit
            {
                UnitIdentifier = e.UnitId,
                CourseIdentifier = e.AggregateIdentifier,
                OrganizationIdentifier = transaction.Course.Entity.OrganizationIdentifier,
                UnitAsset = e.UnitAsset,
                UnitName = e.UnitName,
                UnitSequence = unitAggregateState.UnitSequence,
                UnitIsAdaptive = Unit.Defaults.IsAdaptive,
                Created = e.ChangeTime,
                CreatedBy = e.OriginUser
            };

            ModifyTimestamp(transaction.Course.Entity, unit, null, null, e.ChangeTime, e.OriginUser);

            transaction.Units.Add(e.UnitId, new EntityState<QUnit>(EntityOperation.Insert, unit, unit.OrganizationIdentifier, e.UnitId, ContentContainerType.Unit, e.UnitContent));

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void ModifyUnit(Change e, Guid unitId, ContentContainer content, Action<QUnit> action)
        {
            var unitAggregateState = ((DomainCourseState)e.AggregateState).GetUnit(unitId);
            if (unitAggregateState == null) // Make sure the Unit was not removed from the aggregate when we use multiple changes
                return;

            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unitState) = GetUnitState(transactionId, e.AggregateIdentifier, unitId, EntityOperation.Modify);
            if (unitState.Entity != null)
            {
                if (content != null)
                    unitState.Content = content;

                ModifyTimestamp(transaction.Course.Entity, unitState.Entity, null, null, e.ChangeTime, e.OriginUser);

                action?.Invoke(unitState.Entity);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void RemoveUnit(CourseUnitRemoved e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unitState) = GetUnitState(transactionId, e.AggregateIdentifier, e.UnitId, EntityOperation.Remove);
            if (unitState.Entity != null)
            {
                ModifyTimestamp(transaction.Course.Entity, null, null, null, e.ChangeTime, e.OriginUser);
                IgnoreModules(transaction, e.UnitId);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        private (TransactionState, EntityState<QUnit>) GetUnitState(Guid transactionId, Guid courseId, Guid unitId, EntityOperation operation)
        {
            var transaction = GetTransaction(transactionId, courseId);
            if (transaction.Course.Entity == null)
                return (transaction, null);

            if (!transaction.Units.TryGetValue(unitId, out var unitState))
            {
                using (var db = CreateContext())
                {
                    var unit = db.QUnits.Where(x => x.UnitIdentifier == unitId).FirstOrDefault();
                    transaction.Units.Add(unitId, unitState = new EntityState<QUnit>(
                        operation,
                        unit,
                        unit?.OrganizationIdentifier ?? Guid.Empty,
                        unitId,
                        ContentContainerType.Unit,
                        null
                    ));
                }
            }
            else if (unitState.Operation != EntityOperation.Ignore && unitState.Operation != operation)
            {
                if (operation == EntityOperation.Remove)
                {
                    var newEntity = unitState.Operation != EntityOperation.Insert ? unitState.Entity : null;
                    transaction.Units[unitId] = unitState = new EntityState<QUnit>(EntityOperation.Remove, newEntity);
                }
                else if (unitState.Operation != EntityOperation.Insert || operation != EntityOperation.Modify)
                    throw new ArgumentException($"Invalid operation: {operation}");
            }

            return (transaction, unitState);
        }

        #endregion

        #region Modules

        public void InsertModule(CourseModuleAdded e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unitState) = GetUnitState(transactionId, e.AggregateIdentifier, e.UnitId, EntityOperation.Modify);
            if (transaction.Course.Entity == null)
                throw new ArgumentException($"QCourse does not exist: {e.AggregateIdentifier}");

            if (transaction.Modules.ContainsKey(e.ModuleId))
                throw new ArgumentException("Invalid change: CourseUnitAdded");

            var moduleAggregateState = ((DomainCourseState)e.AggregateState).GetModule(e.ModuleId);
            if (moduleAggregateState == null) // Make sure the Unit was not removed from the aggregate when we use multiple changes
            {
                transaction.Modules.Add(e.ModuleId, new EntityState<QModule>(EntityOperation.Ignore, null));
                return;
            }

            if (unitState.Entity == null)
                throw new ArgumentException($"QUnit does not exist: {e.UnitId}");

            var module = new QModule
            {
                ModuleIdentifier = e.ModuleId,
                UnitIdentifier = e.UnitId,
                OrganizationIdentifier = transaction.Course.Entity.OrganizationIdentifier,
                ModuleAsset = e.ModuleAsset,
                ModuleName = e.ModuleName,
                ModuleSequence = moduleAggregateState.ModuleSequence,
                ModuleIsAdaptive = Module.Defaults.IsAdaptive,
                Created = e.ChangeTime,
                CreatedBy = e.OriginUser
            };

            ModifyTimestamp(transaction.Course.Entity, unitState.Entity, module, null, e.ChangeTime, e.OriginUser);

            transaction.Modules.Add(e.ModuleId, new EntityState<QModule>(
                EntityOperation.Insert,
                module,
                module.OrganizationIdentifier,
                e.ModuleId,
                ContentContainerType.Module,
                e.ModuleContent
            ));

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void ModifyModule(Change e, Guid moduleId, ContentContainer content, Action<QModule> action)
        {
            var moduleAggregateState = ((DomainCourseState)e.AggregateState).GetModule(moduleId);
            if (moduleAggregateState == null) // Make sure the Unit was not removed from the aggregate when we use multiple changes
                return;

            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unit, moduleState) = GetModuleState(transactionId, e.AggregateIdentifier, moduleId, EntityOperation.Modify);
            if (moduleState.Entity != null)
            {
                if (content != null)
                    moduleState.Content = content;

                ModifyTimestamp(transaction.Course.Entity, unit, moduleState.Entity, null, e.ChangeTime, e.OriginUser);

                action?.Invoke(moduleState.Entity);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void RemoveModule(CourseModuleRemoved e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unit, moduleState) = GetModuleState(transactionId, e.AggregateIdentifier, e.ModuleId, EntityOperation.Remove);
            if (moduleState.Entity != null)
            {
                ModifyTimestamp(transaction.Course.Entity, unit, null, null, e.ChangeTime, e.OriginUser);
                IgnoreActivities(transaction, e.ModuleId);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        private void IgnoreModules(TransactionState transaction, Guid unitId)
        {
            var ignoreList = new List<Guid>();

            foreach (var moduleState in transaction.Modules.Values)
            {
                if (moduleState.Entity != null
                    && moduleState.Entity.UnitIdentifier == unitId
                    && moduleState.Operation != EntityOperation.Ignore
                    )
                {
                    ignoreList.Add(moduleState.Entity.ModuleIdentifier);
                }
            }

            foreach (var id in ignoreList)
            {
                transaction.Modules[id] = new EntityState<QModule>(EntityOperation.Ignore, null);

                IgnoreActivities(transaction, id);
            }
        }

        private (TransactionState, QUnit, EntityState<QModule>) GetModuleState(Guid transactionId, Guid courseId, Guid moduleId, EntityOperation operation)
        {
            var transaction = GetTransaction(transactionId, courseId);
            if (transaction.Course.Entity == null)
                return (transaction, null, null);

            if (!transaction.Modules.TryGetValue(moduleId, out var moduleState))
            {
                using (var db = CreateContext())
                {
                    var module = db.QModules.Where(x => x.ModuleIdentifier == moduleId).FirstOrDefault();
                    transaction.Modules.Add(moduleId, moduleState = new EntityState<QModule>(
                        operation,
                        module,
                        module?.OrganizationIdentifier ?? Guid.Empty,
                        moduleId,
                        ContentContainerType.Module,
                        null
                    ));
                }
            }
            else if (moduleState.Operation != EntityOperation.Ignore && moduleState.Operation != operation)
            {
                if (operation == EntityOperation.Remove)
                {
                    var newEntity = moduleState.Operation != EntityOperation.Insert ? moduleState.Entity : null;
                    transaction.Modules[moduleId] = moduleState = new EntityState<QModule>(EntityOperation.Remove, newEntity);
                }
                else if (moduleState.Operation != EntityOperation.Insert || operation != EntityOperation.Modify)
                    throw new ArgumentException($"Invalid operation: {operation}");
            }

            QUnit unit;

            if (moduleState.Entity != null)
            {
                var (_, unitState) = GetUnitState(transactionId, courseId, moduleState.Entity.UnitIdentifier, EntityOperation.Modify);
                unit = unitState.Entity;
            }
            else
                unit = null;

            return (transaction, unit, moduleState);
        }

        #endregion

        #region Activities

        public void InsertActivity(CourseActivityAdded e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unit, moduleState) = GetModuleState(transactionId, e.AggregateIdentifier, e.ModuleId, EntityOperation.Modify);
            if (transaction.Course.Entity == null)
                throw new ArgumentException($"QCourse does not exist: {e.AggregateIdentifier}");

            if (transaction.Activities.ContainsKey(e.ActivityId))
                throw new ArgumentException("Invalid change: CourseActivityAdded");

            var activityAggregateState = ((DomainCourseState)e.AggregateState).GetActivity(e.ActivityId);
            if (activityAggregateState == null) // Make sure the Activity was not removed from the aggregate when we use multiple changes
            {
                transaction.Activities.Add(e.ActivityId, new EntityState<QActivity>(EntityOperation.Ignore, null));
                return;
            }

            if (moduleState.Entity == null)
                throw new ArgumentException($"QModule does not exist: {e.ModuleId}");

            var activityState = ((DomainCourseState)e.AggregateState).GetActivity(e.ActivityId);

            var activity = new QActivity
            {
                ActivityIdentifier = e.ActivityId,
                ModuleIdentifier = e.ModuleId,
                OrganizationIdentifier = transaction.Course.Entity.OrganizationIdentifier,
                ActivityType = e.ActivityType.ToString(),
                ActivityAsset = e.ActivityAsset,
                ActivityName = e.ActivityName,
                ActivitySequence = activityState.GetIntValue(ActivityField.ActivitySequence).Value,
                ActivityIsMultilingual = Activity.Defaults.IsMultilingual,
                ActivityIsAdaptive = Activity.Defaults.IsAdaptive,
                ActivityIsDispatch = Activity.Defaults.IsDispatch,
                Created = e.ChangeTime,
                CreatedBy = e.OriginUser
            };

            ModifyTimestamp(transaction.Course.Entity, unit, moduleState.Entity, null, e.ChangeTime, e.OriginUser);

            transaction.Activities.Add(e.ActivityId, new EntityState<QActivity>(
                EntityOperation.Insert,
                activity,
                activity.OrganizationIdentifier,
                e.ActivityId,
                ContentContainerType.Activity,
                e.ActivityContent
            ));

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void ModifyActivity(Change e, Guid activityId, ContentContainer content, Action<QActivity> action)
        {
            var activityAggregateState = ((DomainCourseState)e.AggregateState).GetActivity(activityId);
            if (activityAggregateState == null) // Make sure the Activity was not removed from the aggregate when we use multiple changes
                return;

            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unit, module, activityState) = GetActivityState(transactionId, e.AggregateIdentifier, activityId, EntityOperation.Modify);
            if (activityState.Entity != null)
            {
                if (content != null)
                    activityState.Content = content;

                ModifyTimestamp(transaction.Course.Entity, unit, module, activityState.Entity, e.ChangeTime, e.OriginUser);

                action?.Invoke(activityState.Entity);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void RemoveActivity(CourseActivityRemoved e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var (transaction, unit, module, activityState) = GetActivityState(transactionId, e.AggregateIdentifier, e.ActivityId, EntityOperation.Remove);
            if (activityState.Entity != null)
            {
                ModifyTimestamp(transaction.Course.Entity, unit, module, null, e.ChangeTime, e.OriginUser);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        private void IgnoreActivities(TransactionState transaction, Guid moduleId)
        {
            var ignoreList = new List<Guid>();

            foreach (var activityState in transaction.Activities.Values)
            {
                if (activityState.Entity != null
                    && activityState.Entity.ModuleIdentifier == moduleId
                    && activityState.Operation != EntityOperation.Ignore
                    )
                {
                    ignoreList.Add(activityState.Entity.ActivityIdentifier);
                }
            }

            foreach (var id in ignoreList)
                transaction.Activities[id] = new EntityState<QActivity>(EntityOperation.Ignore, null);
        }

        private (TransactionState, QUnit, QModule, EntityState<QActivity>) GetActivityState(Guid transactionId, Guid courseId, Guid activityId, EntityOperation operation)
        {
            var transaction = GetTransaction(transactionId, courseId);
            if (transaction.Course.Entity == null)
                return (transaction, null, null, null);

            if (!transaction.Activities.TryGetValue(activityId, out var activityState))
            {
                using (var db = CreateContext())
                {
                    var activity = db.QActivities.Where(x => x.ActivityIdentifier == activityId).FirstOrDefault();
                    transaction.Activities.Add(activityId, activityState = new EntityState<QActivity>(
                        operation,
                        activity,
                        activity?.OrganizationIdentifier ?? Guid.Empty,
                        activityId,
                        ContentContainerType.Activity,
                        null
                    ));
                }
            }
            else if (activityState.Operation != EntityOperation.Ignore && activityState.Operation != operation)
            {
                if (operation == EntityOperation.Remove)
                {
                    var newEntity = activityState.Operation != EntityOperation.Insert ? activityState.Entity : null;
                    transaction.Activities[activityId] = activityState = new EntityState<QActivity>(EntityOperation.Remove, newEntity);
                }
                else if (activityState.Operation != EntityOperation.Insert || operation != EntityOperation.Modify)
                    throw new ArgumentException($"Invalid operation: {operation}");
            }

            QModule module;
            QUnit unit;

            if (activityState.Entity != null)
            {
                var (_, unitEntity, moduleState) = GetModuleState(transactionId, courseId, activityState.Entity.ModuleIdentifier, EntityOperation.Modify);
                unit = unitEntity;
                module = moduleState.Entity;
            }
            else
            {
                unit = null;
                module = null;
            }

            return (transaction, unit, module, activityState);
        }

        #endregion

        private static void ModifyTimestamp(QCourse course, QUnit unit, QModule module, QActivity activity, DateTimeOffset modified, Guid modifiedBy)
        {
            if (course != null)
            {
                course.Modified = modified;
                course.ModifiedBy = modifiedBy;
            }

            if (unit != null)
            {
                unit.Modified = modified;
                unit.ModifiedBy = modifiedBy;
            }

            if (module != null)
            {
                module.Modified = modified;
                module.ModifiedBy = modifiedBy;
            }

            if (activity != null)
            {
                activity.Modified = modified;
                activity.ModifiedBy = modifiedBy;
            }
        }

        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext();
        }
    }
}
