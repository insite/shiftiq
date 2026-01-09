using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Sites.Read;
using InSite.Domain.Sites.Sites;

namespace InSite.Application.Sites.Write
{
    public class SiteCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        private readonly IPageSearch _pageSearch;
        public SiteCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository, IPageSearch pageSearch)
        {
            _publisher = publisher;
            _repository = repository;
            _pageSearch = pageSearch;

            commander.Subscribe<CreateSite>(Handle);

            commander.Subscribe<ChangeSiteTitle>(Handle);
            commander.Subscribe<ChangeSiteType>(Handle);
            commander.Subscribe<ChangeSiteDomain>(Handle);
            commander.Subscribe<ChangeSiteConfiguration>(Handle);
            commander.Subscribe<ChangeSiteContent>(Handle);

            commander.Subscribe<DeleteSite>(Handle);
        }

        private void Commit(SiteAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(CreateSite c)
        {
            var aggregate = new SiteAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.CreateSite(c.Tenant, c.Author, null, c.Domain, c.Title, null);
            Commit(aggregate, c);
        }

        public void Handle(ChangeSiteContent c)
        {
            var aggregate = _repository.Get<SiteAggregate>(c.AggregateIdentifier);
            aggregate.ChangeSiteContent(c.Content);
            Commit(aggregate, c);
        }

        public void Handle(ChangeSiteTitle c)
        {
            var aggregate = _repository.Get<SiteAggregate>(c.AggregateIdentifier);
            aggregate.ChangeSiteTitle(c.Title);
            Commit(aggregate, c);
        }
        public void Handle(ChangeSiteType c)
        {
            var aggregate = _repository.Get<SiteAggregate>(c.AggregateIdentifier);
            aggregate.ChangeSiteType(c.Type);
            Commit(aggregate, c);
        }
        public void Handle(ChangeSiteDomain c)
        {
            var aggregate = _repository.Get<SiteAggregate>(c.AggregateIdentifier);
            aggregate.ChangeSiteDomain(c.Domain);
            Commit(aggregate, c);
        }
        public void Handle(ChangeSiteConfiguration c)
        {
            var aggregate = _repository.Get<SiteAggregate>(c.AggregateIdentifier);
            aggregate.ChangeSiteConfiguration(c.Configuration);
            Commit(aggregate, c);
        }

        public void Handle(DeleteSite c)
        {
            if (_pageSearch.Exists(x => x.SiteIdentifier == c.AggregateIdentifier))
                throw new SiteException($"Site '{c.AggregateIdentifier}' has referenced pages and therefore cannot be deleted");

            _repository.LockAndRun<SiteAggregate>(c.AggregateIdentifier, (aggregate) =>
            {
                aggregate.DeleteSite();
                Commit(aggregate, c);
            });
        }
    }
}
