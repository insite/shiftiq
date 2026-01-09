using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class QRubricStore : IRubricStore
    {
        #region Constants

        private const int MaxRubricTitleLength = 100;
        private const int MaxRubricDescriptionLength = 800;

        private const int MaxRubricCriterionTitleLength = 100;
        private const int MaxRubricCriterionDescriptionLength = 1700;

        private const int MaxRubricCriterionRatingTitleLength = 100;
        private const int MaxRubricCriterionRatingDescriptionLength = 800;

        #endregion

        #region Transactions

        private enum EntityOperation { Insert, Modify, Remove, Ignore }

        private interface IEntityState
        {
            EntityOperation Operation { get; }
            object EntityObject { get; }
            EntityContent Content { get; }
        }

        private class EntityState<T> : IEntityState
        {
            public EntityOperation Operation { get; }
            public T Entity { get; }
            public EntityContent Content { get; set; }

            public object EntityObject => Entity;

            public EntityState(EntityOperation operation, T entity, Guid organizationId, Guid containerId, string containerType)
            {
                Operation = operation;
                Entity = entity;
                Content = new EntityContent(organizationId, containerId, containerType);
            }
        }

        private class EntityContent
        {
            public Guid OrganizationId { get; }
            public Guid ContainerId { get; }
            public string ContainerType { get; }
            public ContentContainer Data { get; }

            public EntityContent(Guid organizationId, Guid containerId, string containerType)
            {
                OrganizationId = organizationId;
                ContainerId = containerId;
                ContainerType = containerType;
                Data = new ContentContainer();
            }
        }

        private class TransactionState
        {
            public Guid AggregateId { get; }

            public EntityState<QRubric> Rubric { get; set; }
            public Dictionary<Guid, EntityState<QRubricCriterion>> Criteria { get; } = new Dictionary<Guid, EntityState<QRubricCriterion>>();
            public Dictionary<Guid, EntityState<QRubricRating>> Ratings { get; } = new Dictionary<Guid, EntityState<QRubricRating>>();

            public RubricState LastAggregateState { get; set; }
            public bool NeedUpdateRubricPoints { get; set; }
            public bool NeedUpdateCriteriaSequence { get; set; }
            public HashSet<Guid> CriteriaWithModifiedRatingSequence { get; } = new HashSet<Guid>();
            public HashSet<Guid> CriteriaWithModifiedRatingPoints { get; } = new HashSet<Guid>();

            public TransactionState(Guid aggregateId)
            {
                AggregateId = aggregateId;
            }

            public EntityState<QRubric> LoadRubric()
            {
                if (Rubric != null)
                    return Rubric;

                using (var db = CreateContext())
                    return LoadRubricInternal(db);
            }

            public EntityState<QRubric> LoadRubric(InternalDbContext db)
            {
                return Rubric ?? LoadRubricInternal(db);
            }

            private EntityState<QRubric> LoadRubricInternal(InternalDbContext db)
            {
                var rubric = db.QRubrics.Where(x => x.RubricIdentifier == AggregateId).FirstOrDefault();

                if (rubric == null)
                    throw ApplicationError.Create("Rubric not found: {0}", AggregateId);

                Rubric = new EntityState<QRubric>(
                    EntityOperation.Modify, rubric,
                    rubric.OrganizationIdentifier, rubric.RubricIdentifier, ContentContainerType.Rubric);

                return Rubric;
            }

            public EntityState<QRubricCriterion> LoadCriterion(Guid organizationId, Guid criterionId, EntityOperation operation = EntityOperation.Modify)
            {
                if (Criteria.TryGetValue(criterionId, out var result))
                    return result;

                using (var db = CreateContext())
                    return LoadCriterionInternal(db, organizationId, criterionId, operation);
            }

            public EntityState<QRubricCriterion> LoadCriterion(InternalDbContext db, Guid organizationId, Guid criterionId, EntityOperation operation = EntityOperation.Modify)
            {
                return Criteria.TryGetValue(criterionId, out var result)
                    ? result
                    : LoadCriterionInternal(db, organizationId, criterionId, operation);
            }

            private EntityState<QRubricCriterion> LoadCriterionInternal(InternalDbContext db, Guid organizationId, Guid criterionId, EntityOperation operation = EntityOperation.Modify)
            {
                var criterion = db.QRubricCriteria
                    .Where(x => x.RubricIdentifier == AggregateId && x.RubricCriterionIdentifier == criterionId)
                    .FirstOrDefault();

                if (criterion == null)
                    throw ApplicationError.Create("Criterion not found: {0}", criterionId);

                var result = new EntityState<QRubricCriterion>(
                    operation, criterion,
                    organizationId, criterion.RubricCriterionIdentifier, ContentContainerType.RubricCriterion);

                Criteria.Add(criterion.RubricCriterionIdentifier, result);

                return result;
            }

            public EntityState<QRubricRating> LoadRating(Guid organizationId, Guid ratingId, EntityOperation operation = EntityOperation.Modify)
            {
                if (Ratings.TryGetValue(ratingId, out var result))
                    return result;

                using (var db = CreateContext())
                    return LoadRatingInternal(db, organizationId, ratingId, operation);
            }

            public EntityState<QRubricRating> LoadRating(InternalDbContext db, Guid organizationId, Guid ratingId, EntityOperation operation = EntityOperation.Modify)
            {
                return Ratings.TryGetValue(ratingId, out var result)
                    ? result
                    : LoadRatingInternal(db, organizationId, ratingId, operation);
            }

            private EntityState<QRubricRating> LoadRatingInternal(InternalDbContext db, Guid organizationId, Guid ratingId, EntityOperation operation = EntityOperation.Modify)
            {
                var rating = db.QRubricRatings
                    .Where(x => x.RubricCriterion.RubricIdentifier == AggregateId && x.RubricRatingIdentifier == ratingId)
                    .FirstOrDefault();

                if (rating == null)
                    throw ApplicationError.Create("Raing not found: {0}", ratingId);

                var result = new EntityState<QRubricRating>(
                    operation, rating,
                    organizationId, rating.RubricRatingIdentifier, ContentContainerType.RubricRating);

                Ratings.Add(rating.RubricRatingIdentifier, result);

                return result;
            }

            public bool IsCriteriaRemoved(Guid criterionId)
            {
                return Criteria.TryGetValue(criterionId, out var state) && state.Operation == EntityOperation.Remove;
            }

            public bool IsRatingRemoved(Guid ratingId)
            {
                return Ratings.TryGetValue(ratingId, out var state) && state.Operation == EntityOperation.Remove;
            }
        }

        private readonly ConcurrentDictionary<Guid, TransactionState> _transactions = new ConcurrentDictionary<Guid, TransactionState>();
        private readonly IContentStore _contentStore;

        public QRubricStore(IContentStore contentStore)
        {
            _contentStore = contentStore;
        }

        public Guid StartTransaction(Guid aggregateId)
        {
            var transactionId = Guid.NewGuid();

            if (!_transactions.TryAdd(transactionId, new TransactionState(aggregateId)))
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
                if (transaction.Rubric == null || !RemoveRubric(db, transaction.Rubric))
                {
                    RemoveCriteria(db, transaction.Criteria.Values);

                    PreCommitActions(db, transaction);

                    if (transaction.Rubric != null)
                        SaveEntity(db, transaction.Rubric);

                    SaveEntities(db, transaction.Ratings.Values);
                    SaveEntities(db, transaction.Criteria.Values);

                    db.SaveChanges();
                }
            }

            if (transaction.Rubric != null)
                SaveContent(transaction.Rubric);

            SaveContents(transaction.Criteria.Values);
            SaveContents(transaction.Ratings.Values);

            _transactions.TryRemove(transactionId, out _);
        }

        private TransactionState GetTransaction(Guid transactionId, Guid aggregateId, RubricState state)
        {
            var transaction = _transactions[transactionId];

            if (transaction.AggregateId != aggregateId)
                throw new ArgumentException($"Transaction ${transactionId} stores rubric ${transaction.AggregateId}, but was requested ${aggregateId}");

            transaction.LastAggregateState = state;

            return transaction;
        }

        private void SaveEntities(InternalDbContext db, IEnumerable<IEntityState> entityStates)
        {
            foreach (var entityState in entityStates)
                SaveEntity(db, entityState);
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
                    if (entityState.EntityObject is QRubricRating)
                        db.Entry(entityState.EntityObject).State = EntityState.Deleted;
                    break;
                default:
                    throw new ArgumentException($"Unsupported operation: {entityState.Operation}");
            }
        }

        private void SaveContent(IEntityState entityState)
        {
            if (entityState.Content?.Data == null)
                return;

            switch (entityState.Operation)
            {
                case EntityOperation.Insert:
                case EntityOperation.Modify:
                    _contentStore.SaveContainer(entityState.Content.OrganizationId, entityState.Content.ContainerType, entityState.Content.ContainerId, entityState.Content.Data);
                    break;
                case EntityOperation.Remove:
                    _contentStore.DeleteContainer(entityState.Content.ContainerId);
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

        private bool RemoveRubric(InternalDbContext db, EntityState<QRubric> rubricState)
        {
            if (rubricState.Operation != EntityOperation.Remove)
                return false;

            db.Database.ExecuteSqlCommand(
                "records.DeleteRubric @RubricId",
                new SqlParameter("RubricId", rubricState.Entity.RubricIdentifier));

            return true;
        }

        private void RemoveCriteria(InternalDbContext db, IEnumerable<EntityState<QRubricCriterion>> criterionStates)
        {
            foreach (var criterionState in criterionStates)
            {
                if (criterionState.Operation == EntityOperation.Remove)
                    db.Database.ExecuteSqlCommand(
                        "records.DeleteRubricCriterion @RubricCriterionId",
                        new SqlParameter("RubricCriterionId", criterionState.Entity.RubricCriterionIdentifier));
            }
        }

        private void PreCommitActions(InternalDbContext db, TransactionState transaction)
        {
            var state = transaction.LastAggregateState;
            if (state == null)
                return;

            UpdateCriteriaSequences(db, transaction, state);
            UpdateRatingsSequences(db, transaction, state);
            UpdateCriterionPoints(db, transaction, state);

            if (transaction.NeedUpdateRubricPoints || transaction.CriteriaWithModifiedRatingPoints.Count > 0)
            {
                var rubric = transaction.LoadRubric(db);
                rubric.Entity.RubricPoints = state.Points;
            }

            RemoveOrphanCriteria(db, transaction, state);
            RemoveOrphanRatings(db, transaction, state);
        }

        private void UpdateCriteriaSequences(InternalDbContext db, TransactionState transaction, RubricState state)
        {
            if (!transaction.NeedUpdateCriteriaSequence)
                return;

            var sequence = 1;

            foreach (var criterionModel in state.Criteria)
            {
                var criterionId = criterionModel.Id;
                if (transaction.IsCriteriaRemoved(criterionId))
                    continue;

                var criterion = transaction.LoadCriterion(db, state.OrganizationId, criterionId);
                if (criterion.Entity != null)
                    criterion.Entity.CriterionSequence = sequence++;
            }
        }

        private void UpdateRatingsSequences(InternalDbContext db, TransactionState transaction, RubricState state)
        {
            foreach (var criterionId in transaction.CriteriaWithModifiedRatingSequence)
            {
                var criterionState = state.FindCriterion(criterionId);
                if (criterionState == null)
                    continue;

                var sequence = 1;

                foreach (var ratingModel in criterionState.Ratings)
                {
                    var ratingId = ratingModel.Id;
                    if (transaction.IsRatingRemoved(ratingId))
                        continue;

                    var rating = transaction.LoadRating(db, state.OrganizationId, ratingId);
                    if (rating.Entity != null)
                        rating.Entity.RatingSequence = sequence++;
                }
            }
        }

        private void UpdateCriterionPoints(InternalDbContext db, TransactionState transaction, RubricState state)
        {
            foreach (var criterionId in transaction.CriteriaWithModifiedRatingPoints)
            {
                if (transaction.IsCriteriaRemoved(criterionId))
                    continue;

                var criterionModel = state.FindCriterion(criterionId);
                if (criterionModel == null)
                    continue;

                var criterion = transaction.LoadCriterion(db, state.OrganizationId, criterionId);
                if (criterion.Entity != null)
                    criterion.Entity.CriterionPoints = criterionModel.Points;
            }
        }

        private void RemoveOrphanCriteria(InternalDbContext db, TransactionState transaction, RubricState state)
        {
            var existCriteriaIds = state.Criteria
                .Select(x => x.Id)
                .ToHashSet();
            var removeCriteriaIds = transaction.Criteria
                .Where(x => x.Value.Operation == EntityOperation.Insert
                         && !existCriteriaIds.Contains(x.Value.Entity.RubricCriterionIdentifier))
                .Select(x => x.Key)
                .ToList();

            foreach (var criteriaId in removeCriteriaIds)
                transaction.Criteria.Remove(criteriaId);
        }

        private void RemoveOrphanRatings(InternalDbContext db, TransactionState transaction, RubricState state)
        {
            var existRatingIds = state.Criteria
                .SelectMany(x => x.Ratings)
                .Select(x => x.Id)
                .ToHashSet();
            var removeRatingIds = transaction.Ratings
                .Where(x => x.Value.Operation == EntityOperation.Insert
                         && !existRatingIds.Contains(x.Value.Entity.RubricRatingIdentifier))
                .Select(x => x.Key)
                .ToList();

            foreach (var ratingId in removeRatingIds)
                transaction.Ratings.Remove(ratingId);
        }

        #endregion

        #region Rubric

        public void Insert(RubricCreated e)
        {
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);

            var transaction = _transactions[transactionId];
            if (transaction.Rubric != null)
                throw new ArgumentException("Invalid change: RubricCreated");

            var entity = new QRubric
            {
                RubricIdentifier = e.AggregateIdentifier,
                OrganizationIdentifier = e.OriginOrganization,
                RubricTitle = e.RubricTitle.MaxLength(100),
                Created = e.ChangeTime,
                CreatedBy = e.OriginUser
            };

            SetTimestamp(entity, e);

            transaction.Rubric = new EntityState<QRubric>(
                EntityOperation.Insert, entity,
                entity.OrganizationIdentifier, entity.RubricIdentifier, ContentContainerType.Rubric);
            transaction.Rubric.Content.Data.Title.Text.Default = e.RubricTitle;

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Delete(RubricDeleted e)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);

            transaction.Rubric = new EntityState<QRubric>(
                EntityOperation.Remove,
                new QRubric { RubricIdentifier = e.AggregateIdentifier },
                e.OriginOrganization, e.AggregateIdentifier, ContentContainerType.Rubric);

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Update(RubricRenamed e) => UpdateRubric(e,
            entity => entity.RubricTitle = e.RubricTitle.MaxLength(MaxRubricTitleLength),
            content => content.Title.Text.Default = e.RubricTitle
        );

        public void Update(RubricDescribed e) => UpdateRubric(e,
            entity => entity.RubricDescription = e.RubricDescription.MaxLength(MaxRubricDescriptionLength),
            content => content.Description.Text.Default = e.RubricDescription);

        public void Update(RubricTimestampModified e) => UpdateRubric(e, entity =>
        {
            entity.Created = e.Created;
            entity.CreatedBy = e.CreatedBy;
            entity.Modified = e.Modified;
            entity.ModifiedBy = e.ModifiedBy;
        });

        public void Update(RubricTranslated e) => UpdateRubric(e,
            entity =>
            {
                var state = (RubricState)e.AggregateState;
                entity.RubricTitle = state.Content.Title.Text.Default.MaxLength(MaxRubricTitleLength);
                entity.RubricDescription = state.Content.Description.Text.Default.MaxLength(MaxRubricDescriptionLength);
            },
            content => content.Set(e.Content, ContentContainer.SetNullAction.Set));

        private void UpdateRubric(Change e, Action<QRubric> updateEntity, Action<ContentContainer> updateContent = null)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);
            var entityState = transaction.LoadRubric();

            if (entityState.Entity != null)
            {
                SetTimestamp(entityState.Entity, e);

                updateEntity?.Invoke(entityState.Entity);
                updateContent?.Invoke(entityState.Content.Data);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        #endregion

        #region Criterion

        public void Insert(RubricCriterionAdded e)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);

            var entity = new QRubricCriterion
            {
                RubricIdentifier = e.AggregateIdentifier,
                RubricCriterionIdentifier = e.RubricCriterionId,
                CriterionTitle = e.CriterionTitle.MaxLength(100),
                IsRange = e.IsRange
            };

            var entityState = new EntityState<QRubricCriterion>(
                EntityOperation.Insert, entity,
                state.OrganizationId, entity.RubricCriterionIdentifier, ContentContainerType.RubricCriterion);
            entityState.Content.Data.Title.Text.Default = e.CriterionTitle;

            transaction.Criteria.Add(entity.RubricCriterionIdentifier, entityState);

            transaction.NeedUpdateCriteriaSequence = true;

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Delete(RubricCriterionRemoved e)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);

            if (!transaction.Criteria.TryGetValue(e.RubricCriterionId, out var existCriterionState))
            {
                transaction.LoadCriterion(state.OrganizationId, e.RubricCriterionId, EntityOperation.Remove);
            }
            else if (existCriterionState.Operation == EntityOperation.Insert)
            {
                transaction.Criteria.Remove(e.RubricCriterionId);
            }
            else
            {
                transaction.Criteria[e.RubricCriterionId] = new EntityState<QRubricCriterion>(
                    EntityOperation.Remove, new QRubricCriterion
                    {
                        RubricIdentifier = e.AggregateIdentifier,
                        RubricCriterionIdentifier = e.RubricCriterionId
                    },
                    state.OrganizationId, e.RubricCriterionId, ContentContainerType.RubricCriterion);
            }

            transaction.NeedUpdateRubricPoints = true;
            transaction.NeedUpdateCriteriaSequence = true;

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Update(RubricCriterionRenamed e) => UpdateCriterion(e, e.RubricCriterionId,
            entity => entity.CriterionTitle = e.CriterionTitle.MaxLength(MaxRubricCriterionTitleLength),
            content => content.Title.Text.Default = e.CriterionTitle);

        public void Update(RubricCriterionDescribed e) => UpdateCriterion(e, e.RubricCriterionId,
            entity => entity.CriterionDescription = e.CriterionDescription.MaxLength(MaxRubricCriterionDescriptionLength),
            content => content.Description.Text.Default = e.CriterionDescription);

        public void Update(RubricCriterionTranslated e) => UpdateCriterion(e, e.RubricCriterionId,
            entity =>
            {
                var state = (RubricState)e.AggregateState;
                var criterion = state.FindCriterion(e.RubricCriterionId);

                entity.CriterionTitle = criterion.Content.Title.Text.Default.MaxLength(MaxRubricCriterionTitleLength);
                entity.CriterionDescription = criterion.Content.Description.Text.Default.MaxLength(MaxRubricCriterionDescriptionLength);
            },
            content => content.Set(e.Content, ContentContainer.SetNullAction.Set));

        public void Update(RubricCriterionIsRangeModified e) => UpdateCriterion(e, e.RubricCriterionId, entity => entity.IsRange = e.IsRange);

        private void UpdateCriterion(Change e, Guid criterionId, Action<QRubricCriterion> updateEntity, Action<ContentContainer> updateContent = null)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);
            var entityState = transaction.LoadCriterion(state.OrganizationId, criterionId);

            if (entityState.Entity != null)
            {
                updateEntity?.Invoke(entityState.Entity);
                updateContent?.Invoke(entityState.Content.Data);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        #endregion

        #region Rating

        public void Insert(RubricCriterionRatingAdded e)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);

            var entity = new QRubricRating
            {
                RubricCriterionIdentifier = e.RubricCriterionId,
                RubricRatingIdentifier = e.RubricRatingId,
                RatingTitle = e.RatingTitle.MaxLength(100),
                RatingPoints = e.RatingPoints,
            };

            var entityState = new EntityState<QRubricRating>(
                EntityOperation.Insert, entity,
                state.OrganizationId, entity.RubricRatingIdentifier, ContentContainerType.RubricRating);
            entityState.Content.Data.Title.Text.Default = e.RatingTitle;

            transaction.Ratings.Add(entity.RubricRatingIdentifier, entityState);

            transaction.CriteriaWithModifiedRatingSequence.Add(entity.RubricCriterionIdentifier);
            transaction.CriteriaWithModifiedRatingPoints.Add(entity.RubricCriterionIdentifier);
            transaction.NeedUpdateRubricPoints = true;

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Delete(RubricCriterionRatingRemoved e)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);

            Guid criterionId;

            if (!transaction.Ratings.TryGetValue(e.RubricRatingId, out var existRatingState))
            {
                var rating = transaction.LoadRating(state.OrganizationId, e.RubricRatingId, EntityOperation.Remove);
                criterionId = rating.Entity.RubricCriterionIdentifier;
            }
            else if (existRatingState.Operation == EntityOperation.Insert)
            {
                criterionId = existRatingState.Entity.RubricCriterionIdentifier;
                transaction.Ratings.Remove(e.RubricRatingId);
            }
            else
            {
                criterionId = existRatingState.Entity.RubricCriterionIdentifier;
                transaction.Ratings[e.RubricRatingId] = new EntityState<QRubricRating>(
                    EntityOperation.Remove,
                    new QRubricRating { RubricRatingIdentifier = e.RubricRatingId },
                    state.OrganizationId, e.RubricRatingId, ContentContainerType.RubricRating);
            }

            transaction.CriteriaWithModifiedRatingPoints.Add(criterionId);
            transaction.CriteriaWithModifiedRatingSequence.Add(criterionId);
            transaction.NeedUpdateRubricPoints = true;

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        public void Update(RubricCriterionRatingRenamed e) => UpdateRating(e, e.RubricRatingId,
            (_, entity) => entity.RatingTitle = e.RatingTitle.MaxLength(MaxRubricCriterionRatingTitleLength),
            content => content.Title.Text.Default = e.RatingTitle);

        public void Update(RubricCriterionRatingDescribed e) => UpdateRating(e, e.RubricRatingId,
            (_, entity) => entity.RatingDescription = e.RatingDescription.MaxLength(MaxRubricCriterionRatingDescriptionLength),
            content => content.Description.Text.Default = e.RatingDescription);

        public void Update(RubricCriterionRatingPointsModified e) => UpdateRating(e, e.RubricRatingId, (transaction, entity) =>
        {
            var state = (RubricState)e.AggregateState;
            var rating = state.FindRating(e.RubricRatingId);

            entity.RatingPoints = e.RatingPoints;

            transaction.CriteriaWithModifiedRatingPoints.Add(entity.RubricCriterionIdentifier);
            transaction.NeedUpdateRubricPoints = true;
        });

        public void Update(RubricCriterionRatingTranslated e) => UpdateRating(e, e.RubricRatingId,
            (_, entity) =>
            {
                var state = (RubricState)e.AggregateState;
                var rating = state.FindRating(e.RubricRatingId);

                entity.RatingTitle = rating.Content.Title.Text.Default.MaxLength(MaxRubricCriterionRatingTitleLength);
                entity.RatingDescription = rating.Content.Description.Text.Default.MaxLength(MaxRubricCriterionRatingDescriptionLength);
            },
            content => content.Set(e.Content, ContentContainer.SetNullAction.Set));

        private void UpdateRating(Change e, Guid ratingId, Action<TransactionState, QRubricRating> updateEntity, Action<ContentContainer> updateContent = null)
        {
            var state = (RubricState)e.AggregateState;
            var transactionId = e.ChangeTransactionId ?? StartTransaction(e.AggregateIdentifier);
            var transaction = GetTransaction(transactionId, e.AggregateIdentifier, state);
            var entityState = transaction.LoadRating(state.OrganizationId, ratingId);

            if (entityState.Entity != null)
            {
                updateEntity?.Invoke(transaction, entityState.Entity);
                updateContent?.Invoke(entityState.Content.Data);
            }

            if (e.ChangeTransactionId == null)
                CommitTransaction(transactionId);
        }

        #endregion

        #region Helpers

        private void SetTimestamp(QRubric entity, Change e)
        {
            entity.Modified = e.ChangeTime;
            entity.ModifiedBy = e.OriginUser;
        }

        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext();
        }

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE records.QRubric");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE records.QRubricCriterion");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE records.QRubricRating");
            }
        }

        public void Delete(Guid rubricId)
        {
            using (var db = CreateContext())
            {
                db.Database.ExecuteSqlCommand(
                    "records.DeleteRubric @RubricId",
                    new SqlParameter("RubricId", rubricId));
            }
        }

        #endregion
    }
}
