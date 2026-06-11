using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Periods.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

namespace InSite.Application.Records.Write
{
    public class PeriodCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IRecordSearch _recordSearch;

        public PeriodCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository, IRecordSearch recordSearch)
        {
            _publisher = publisher;
            _repository = repository;
            _recordSearch = recordSearch;

            commander.Subscribe<CreatePeriod>(Handle);
            commander.Subscribe<DeletePeriod>(Handle);
            commander.Subscribe<RenamePeriod>(Handle);
            commander.Subscribe<ReschedulePeriod>(Handle);
        }

        private void Commit(PeriodAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(CreatePeriod c)
        {
            if (c.Start >= c.End)
                throw new PeriodException($"End date must be greater than Start");

            var aggregate = new PeriodAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.Create(c.Tenant, c.Name, c.Start, c.End);
            Commit(aggregate, c);
        }

        public void Handle(DeletePeriod c)
        {
            if (_recordSearch.CountGradebooks(new QGradebookFilter { PeriodIdentifier = c.AggregateIdentifier }) > 0)
                throw new PeriodException($"Period {c.AggregateIdentifier} has associated gradebooks and cannot be deleted");

            _repository.LockAndRun<PeriodAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.Delete();
                Commit(aggregate, c);
            });
        }

        public void Handle(RenamePeriod c)
        {
            _repository.LockAndRun<PeriodAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (c.Name != aggregate.Data.Name)
                {
                    aggregate.Rename(c.Name);
                    Commit(aggregate, c);
                }
            });
        }

        public void Handle(ReschedulePeriod c)
        {
            if (c.Start >= c.End)
                throw new PeriodException($"End date must be greater than Start");

            _repository.LockAndRun<PeriodAggregate>(c.AggregateIdentifier, aggregate =>
            {
                if (c.Start != aggregate.Data.Start || c.End != aggregate.Data.End)
                {
                    aggregate.Reschedule(c.Start, c.End);
                    Commit(aggregate, c);
                }
            });
        }
    }
}
