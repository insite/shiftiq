using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Read;
using InSite.Domain.Events;

using Shift.Constant;

namespace InSite.Persistence.Integration.BCMail
{
    public class CustomDistributionSubscriber
    {
        private readonly IEventSearch _eventSearch;
        private readonly IBCMailServer _bcm;

        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public CustomDistributionSubscriber(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository,
            IBCMailServer bcm, IEventSearch eventSearch)
        {
            var ita = OrganizationIdentifiers.SkilledTradesBC;

            commander.Override<OrderDistribution>(Handle, ita);
            commander.Override<TrackDistribution>(Handle, ita);

            _publisher = publisher;
            _repository = repository;

            _eventSearch = eventSearch;
            _bcm = bcm;
        }

        private void Commit(EventAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(OrderDistribution command)
        {
            var output = _bcm.Create(command.Request, command.Test);

            var aggregate = _repository.Get<EventAggregate>(command.AggregateIdentifier, command.ExpectedVersion);

            aggregate.OrderDistribution(output.Code, output.Status, output.Errors);
            aggregate.TrackDistribution(output.Code, output.Status, output.Errors);

            Commit(aggregate, command);
        }

        public void Handle(TrackDistribution command)
        {
            var @event = _eventSearch.GetEvent(command.AggregateIdentifier);

            if (string.IsNullOrEmpty(@event.DistributionCode))
                return;

            var jobs = _bcm.Status(new[] { @event.DistributionCode }, command.Test);

            var aggregate = _repository.Get<EventAggregate>(command.AggregateIdentifier, command.ExpectedVersion);

            if (jobs.Length == 1)
                aggregate.TrackDistribution(jobs[0].Code, jobs[0].Status, jobs[0].Errors);

            Commit(aggregate, command);
        }
    }
}