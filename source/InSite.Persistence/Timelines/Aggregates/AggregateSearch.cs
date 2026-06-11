using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Logs.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class AggregateSearch : IAggregateSearch
    {
        private readonly IChangeRepository _repository;

        public AggregateSearch(IChangeRepository repository)
        {
            _repository = repository;
        }

        public SerializedAggregate Get(Guid aggregateId)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Aggregates.AsNoTracking()
                    .Where(x => x.AggregateIdentifier == aggregateId)
                    .FirstOrDefault();
            }
        }

        public SerializedAggregate[] Get(IEnumerable<Guid> aggregateIds)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Aggregates.AsNoTracking()
                    .Where(x => aggregateIds.Contains(x.AggregateIdentifier))
                    .ToArray();
            }
        }

        public SerializedAggregate[] GetByClass(Guid organizationId, string[] classes)
        {
            if (classes.IsEmpty())
                return null;

            using (var db = new InternalDbContext(false))
            {
                return db.Aggregates.AsNoTracking()
                    .Where(x => x.OriginOrganization == organizationId && classes.Contains(x.AggregateClass))
                    .ToArray();
            }
        }

        public SerializedAggregate[] GetByType(string type, Guid? organization)
        {
            using (var db = new InternalDbContext(false))
            {
                var query = db.Aggregates.AsNoTracking()
                    .Where(x => x.AggregateType == type);

                if (organization.HasValue)
                    query = query.Where(x => x.OriginOrganization == organization);

                return query.ToArray();
            }
        }

        public SerializedAggregate[] GetByOrganization(Guid organization)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Aggregates
                    .Where(x => x.OriginOrganization == organization)
                    .ToArray();
            }
        }

        public SerializedAggregate[] GetGhosts()
        {
            var select = @"
      SELECT * FROM logs.[Aggregate] WHERE AggregateType = 'Bank'    AND AggregateIdentifier NOT IN (SELECT BankIdentifier FROM banks.QBank)
UNION SELECT * FROM logs.[Aggregate] WHERE AggregateType = 'Event'   AND AggregateIdentifier NOT IN (SELECT EventIdentifier FROM events.QEvent)
UNION SELECT * FROM logs.[Aggregate] WHERE AggregateType = 'Message' AND AggregateIdentifier NOT IN (SELECT MessageIdentifier FROM messages.QMessage)
";
            using (var db = new InternalDbContext(false))
                return db.Database.SqlQuery<SerializedAggregate>(select).ToArray();
        }

        public AggregateState GetState<T>(Guid id) where T : AggregateRoot
        {
            return _repository.Exists<T>(id)
                ? _repository.GetClone<T>(id)?.State
                : null;
        }

        public StateType GetState<AggregateType, StateType>(Guid id)
            where AggregateType : AggregateRoot
            where StateType : AggregateState
        {
            return _repository.Exists<AggregateType>(id)
                ? (StateType)_repository.GetClone<AggregateType>(id)?.State
                : null;
        }
    }
}
