using Shift.Common.Timeline.Changes;

using InSite.Domain.Glossaries;

namespace InSite.Application.Glossaries.Read
{
    public class GlossaryChangeProjector
    {
        private readonly IGlossaryStore _store;

        public GlossaryChangeProjector(IChangeQueue publisher, IGlossaryStore store)
        {
            _store = store;

            publisher.Subscribe<GlossaryInitialized>(Handle);
            publisher.Subscribe<GlossaryInitialized2>(Handle);
            publisher.Subscribe<GlossaryTermProposed>(Handle);
            publisher.Subscribe<GlossaryTermApproved>(Handle);
            publisher.Subscribe<GlossaryTermRevised>(Handle);
            publisher.Subscribe<GlossaryTermRejected>(Handle);
            publisher.Subscribe<GlossaryTermLinked>(Handle);
            publisher.Subscribe<GlossaryTermUnlinked>(Handle);
        }

        public void Handle(GlossaryInitialized e)
            => _store.InitializeGlossary(e.AggregateIdentifier, e.Tenant);

        public void Handle(GlossaryInitialized2 e)
            => _store.InitializeGlossary(e.AggregateIdentifier, e.OriginOrganization);

        public void Handle(GlossaryTermProposed e)
            => _store.ProposeTerm(e.AggregateIdentifier, e.OriginOrganization, e.Identifier, e.Name, e.Content, e.ChangeTime, e.OriginUser);

        public void Handle(GlossaryTermApproved e)
            => _store.ApproveTerm(e.Term, e.ChangeTime, e.OriginUser);

        public void Handle(GlossaryTermRevised e)
            => _store.ReviseTerm(e.Identifier, e.Name, e.Content, e.ChangeTime, e.OriginUser);

        public void Handle(GlossaryTermRejected e)
            => _store.DeleteTerm(e.Term);

        public void Handle(GlossaryTermLinked e)
            => _store.LinkTerm(e.RelationshipId, e.TermId, e.ContainerId, e.ContainerType, e.ContentLabel);

        public void Handle(GlossaryTermUnlinked e)
            => _store.UnlinkTerm(e.RelationshipId);
    }
}