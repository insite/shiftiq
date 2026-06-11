using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Application.Contents.Read;
using InSite.Application.Logs.Read;
using InSite.Domain.Sites.Pages;

using Shift.Constant;

namespace InSite.Application.Sites.Read
{
    public class PageChangeProjector
    {
        private readonly IPageStore _store;
        private readonly IAggregateSearch _aggregates;
        private readonly IContentStore _contentStore;

        public PageChangeProjector(IChangeQueue publisher, IAggregateSearch aggregates, IPageStore store, IContentStore contentStore)
        {
            _store = store;
            _aggregates = aggregates;
            _contentStore = contentStore;

            publisher.Subscribe<PageCreated>(Handle);

            publisher.Subscribe<AuthorNameChanged>(Handle);
            publisher.Subscribe<ContentControlChanged>(Handle);
            publisher.Subscribe<ContentLabelsChanged>(Handle);
            publisher.Subscribe<HookChanged>(Handle);
            publisher.Subscribe<IconChanged>(Handle);
            publisher.Subscribe<NavigationUrlChanged>(Handle);
            publisher.Subscribe<NewTabValueChanged>(Handle);
            publisher.Subscribe<SequenceChanged>(Handle);
            publisher.Subscribe<SlugChanged>(Handle);
            publisher.Subscribe<TitleChanged>(Handle);
            publisher.Subscribe<TypeChanged>(Handle);
            publisher.Subscribe<VisibilityChanged>(Handle);
            publisher.Subscribe<PageContentChanged>(Handle);
            publisher.Subscribe<ParentChanged>(Handle);
            publisher.Subscribe<SiteChanged>(Handle);
            publisher.Subscribe<SurveyChanged>(Handle);
            publisher.Subscribe<CourseChanged>(Handle);
            publisher.Subscribe<PageAssessmentChanged>(Handle);
            publisher.Subscribe<ProgramChanged>(Handle);
            publisher.Subscribe<AuthorDateChanged>(Handle);

            publisher.Subscribe<PageObjectModified>(Handle);

            publisher.Subscribe<PageDeleted>(Handle);
        }

        public void Handle(PageCreated e)
            => _store.InsertPage(e);

        public void Handle(AuthorNameChanged e)
                    => _store.UpdatePage(e);
        public void Handle(ContentControlChanged e)
                    => _store.UpdatePage(e);
        public void Handle(ContentLabelsChanged e)
                    => _store.UpdatePage(e);
        public void Handle(HookChanged e)
                    => _store.UpdatePage(e);
        public void Handle(IconChanged e)
                    => _store.UpdatePage(e);
        public void Handle(NavigationUrlChanged e)
                    => _store.UpdatePage(e);
        public void Handle(NewTabValueChanged e)
                    => _store.UpdatePage(e);
        public void Handle(SequenceChanged e)
                    => _store.UpdatePage(e);
        public void Handle(SlugChanged e)
                    => _store.UpdatePage(e);
        public void Handle(TitleChanged e)
                    => _store.UpdatePage(e);
        public void Handle(TypeChanged e)
                    => _store.UpdatePage(e);
        public void Handle(VisibilityChanged e)
                    => _store.UpdatePage(e);
        public void Handle(PageContentChanged e)
        {
            _contentStore.SaveContainer(e.OriginOrganization, ContentContainerType.WebPage, e.AggregateIdentifier, e.Content);
            _store.UpdatePage(e);
        }
        public void Handle(ParentChanged e)
            => _store.UpdatePage(e);
        public void Handle(SiteChanged e)
                    => _store.UpdatePage(e);
        public void Handle(SurveyChanged e)
            => _store.UpdatePage(e);
        public void Handle(CourseChanged e)
            => _store.UpdatePage(e);
        public void Handle(PageAssessmentChanged e)
            => _store.UpdatePage(e);
        public void Handle(PageObjectModified e)
            => _store.UpdatePage(e);
        public void Handle(ProgramChanged e)
            => _store.UpdatePage(e);
        public void Handle(AuthorDateChanged e)
            => _store.UpdatePage(e);

        public void Handle(PageDeleted e)
            => _store.DeletePage(e);

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear all the existing data in the query store for this projection.
            if (id.HasValue)
                _store.DeleteOne(id.Value);
            else
                _store.DeleteAll();

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Page", id, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Page", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}
