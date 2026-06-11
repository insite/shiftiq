using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Pages.Write;
using InSite.Domain.Sites.Pages;

namespace InSite.Application.Sites.Write
{
    public class PageCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;
        public PageCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<CreatePage>(Handle);

            commander.Subscribe<ChangePageAuthorName>(Handle);
            commander.Subscribe<ChangePageContentControl>(Handle);
            commander.Subscribe<ChangePageContentLabels>(Handle);
            commander.Subscribe<ChangePageHook>(Handle);
            commander.Subscribe<ChangePageIcon>(Handle);
            commander.Subscribe<ChangePageNavigationUrl>(Handle);
            commander.Subscribe<ChangePageNewTabValue>(Handle);
            commander.Subscribe<ChangePageSequence>(Handle);
            commander.Subscribe<ChangePageSlug>(Handle);
            commander.Subscribe<ChangePageTitle>(Handle);
            commander.Subscribe<ChangePageType>(Handle);
            commander.Subscribe<ChangePageVisibility>(Handle);
            commander.Subscribe<ChangePageContent>(Handle);
            commander.Subscribe<ChangePageParent>(Handle);
            commander.Subscribe<ChangePageSite>(Handle);
            commander.Subscribe<ChangePageAuthorDate>(Handle);
            commander.Subscribe<ChangePageSurvey>(Handle);
            commander.Subscribe<ChangePageCourse>(Handle);
            commander.Subscribe<ChangePageProgram>(Handle);
            commander.Subscribe<ChangePageAssessment>(Handle);

            commander.Subscribe<ModifyPageObject>(Handle);

            commander.Subscribe<DeletePage>(Handle);
        }

        private void Commit(PageAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(ChangePageContent c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageContent(c.Content);
            Commit(aggregate, c);
        }

        public void Handle(CreatePage c)
        {
            var aggregate = new PageAggregate { AggregateIdentifier = c.AggregateIdentifier };

            if(c.Site.HasValue)
                aggregate.RootAggregateIdentifier= c.Site.Value;

            aggregate.CreatePage(c.Site, c.ParentPage, c.Tenant, c.Author, c.Title, c.Type, c.Sequence, c.IsHidden, c.IsNewTab);
            Commit(aggregate, c);
        }


        public void Handle(ChangePageAuthorName c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageAuthorName(c.AuthorName);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageContentControl c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageContentControl(c.ContentControl);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageContentLabels c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageContentLabels(c.ContentLabels);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageHook c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageHook(c.Hook);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageIcon c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageIcon(c.Icon);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageNavigationUrl c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageNavigationUrl(c.NavigateUrl);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageNewTabValue c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageNewTabValue(c.IsNewTab);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageSequence c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageSequence(c.Sequence);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageSlug c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageSlug(c.Slug);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageTitle c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageTitle(c.Title);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageType c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageType(c.Type);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageVisibility c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageVisibility(c.IsHidden);
            Commit(aggregate, c);
        }

        public void Handle(ChangePageParent c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageParent(c.Parent);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageSite c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageSite(c.Site);
            Commit(aggregate, c);
        }
        public void Handle(ChangePageSurvey c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageSurvey(c.Survey);
            Commit(aggregate, c);
        }

        public void Handle(ChangePageCourse c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageCourse(c.Course);
            Commit(aggregate, c);
        }

        public void Handle(ChangePageAssessment c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageAssessment(c.Assessment);
            Commit(aggregate, c);
        }

        public void Handle(ChangePageProgram c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageProgram(c.Program);
            Commit(aggregate, c);
        }

        public void Handle(ChangePageAuthorDate c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ChangePageAuthorDate(c.AuthorDate);
            Commit(aggregate, c);
        }

        public void Handle(ModifyPageObject c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.ModifyPageObject(c.Type, c.Object);
            Commit(aggregate, c);
        }

        public void Handle(DeletePage c)
        {
            var aggregate = _repository.Get<PageAggregate>(c.AggregateIdentifier);
            aggregate.DeletePage();
            Commit(aggregate, c);
        }
    }
}
