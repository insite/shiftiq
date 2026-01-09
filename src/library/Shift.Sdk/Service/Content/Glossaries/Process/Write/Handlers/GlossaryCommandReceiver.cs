using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Domain.Glossaries;

namespace InSite.Application.Glossaries.Write
{
    public class GlossaryCommandReceiver
    {
        private readonly IChangeRepository _repository;
        private readonly IChangeQueue _publisher;

        public GlossaryCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _repository = repository;
            _publisher = publisher;

            commander.Subscribe<InitializeGlossary>(Handle);
            commander.Subscribe<ProposeGlossaryTerm>(Handle);
            commander.Subscribe<ApproveGlossaryTerm>(Handle);
            commander.Subscribe<ReviseGlossaryTerm>(Handle);
            commander.Subscribe<RejectGlossaryTerm>(Handle);
            commander.Subscribe<LinkGlossaryTerm>(Handle);
            commander.Subscribe<UnlinkGlossaryTerm>(Handle);
        }

        private void Commit(GlossaryAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(InitializeGlossary c)
        {
            var aggregate = new GlossaryAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.Initialize();
            Commit(aggregate, c);
        }

        public void Handle(ProposeGlossaryTerm c)
        {
            var aggregate = _repository.Get<GlossaryAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ProposeGlossaryTerm(c.Identifier, c.Name, c.Content);
            Commit(aggregate, c);
        }

        public void Handle(ApproveGlossaryTerm c)
        {
            var aggregate = _repository.Get<GlossaryAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ApproveGlossaryTerm(c.Term);
            Commit(aggregate, c);
        }

        public void Handle(ReviseGlossaryTerm c)
        {
            var aggregate = _repository.Get<GlossaryAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReviseGlossaryTerm(c.Identifier, c.Name, c.Content);
            Commit(aggregate, c);
        }

        public void Handle(RejectGlossaryTerm c)
        {
            var aggregate = _repository.Get<GlossaryAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RejectGlossaryTerm(c.Term);
            Commit(aggregate, c);
        }

        public void Handle(LinkGlossaryTerm c)
        {
            var aggregate = _repository.Get<GlossaryAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.LinkGlossaryTerm(c.RelationshipIdentifier, c.TermIdentifier, c.ContainerIdentifier, c.ContainerType, c.ContentLabel);
            Commit(aggregate, c);
        }

        public void Handle(UnlinkGlossaryTerm c)
        {
            var aggregate = _repository.Get<GlossaryAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.UnlinkGlossaryTerm(c.RelationshipIdentifier, c.TermIdentifier);
            Commit(aggregate, c);
        }
    }
}
