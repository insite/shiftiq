using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Application.Contents.Read;
using InSite.Application.Logs.Read;
using InSite.Domain.Sites.Sites;

using Shift.Constant;

namespace InSite.Application.Sites.Read
{
    public class SiteChangeProjector
    {
        private readonly ISiteStore _store;
        private readonly IAggregateSearch _aggregates;
        private readonly IContentStore _contentStore;

        public SiteChangeProjector(IChangeQueue publisher, IAggregateSearch aggregates, ISiteStore store, IContentStore contentStore)
        {
            _store = store;
            _aggregates = aggregates;
            _contentStore = contentStore;

            publisher.Subscribe<SiteCreated>(Handle);

            publisher.Subscribe<SiteTitleChanged>(Handle);
            publisher.Subscribe<SiteConfigurationChanged>(Handle);
            publisher.Subscribe<SiteDomainChanged>(Handle);
            publisher.Subscribe<SiteTypeChanged>(Handle);
            publisher.Subscribe<SiteContentChanged>(Handle);

            publisher.Subscribe<SiteDeleted>(Handle);
        }

        public void Handle(SiteCreated e)
            => _store.InsertSite(e);

        public void Handle(SiteTitleChanged e)
            => _store.UpdateSite(e);
        public void Handle(SiteConfigurationChanged e)
            => _store.UpdateSite(e);
        public void Handle(SiteDomainChanged e)
            => _store.UpdateSite(e);
        public void Handle(SiteTypeChanged e)
            => _store.UpdateSite(e);
        public void Handle(SiteContentChanged e)
        {
            _contentStore.SaveContainer(e.OriginOrganization, ContentContainerType.WebSite, e.AggregateIdentifier, e.Content);
            _store.UpdateSite(e);
        }

        public void Handle(SiteDeleted e)
            => _store.DeleteSite(e);

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear all the existing data in the query store for this projection.
            if (id.HasValue)
                _store.DeleteOne(id.Value);
            else
                _store.DeleteAll();

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Site", id, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Site", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}
