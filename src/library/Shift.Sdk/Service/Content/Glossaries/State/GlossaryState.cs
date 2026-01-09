using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Glossaries
{
    public class Glossary : AggregateState
    {
        public Guid TenantIdentifier { get; set; }

        public List<GlossaryItem> Dictionary;

        public void When(GlossaryInitialized e)
        {
            TenantIdentifier = e.Tenant;
            Dictionary = new List<GlossaryItem>();
        }

        public void When(GlossaryInitialized2 e)
        {
            TenantIdentifier = e.OriginOrganization;
            Dictionary = new List<GlossaryItem>();
        }

        public void When(GlossaryTermProposed e)
        {
            Dictionary.Add(new GlossaryItem { Identifier = e.Identifier, Name = e.Name, Content = e.Content });
        }

        public void When(GlossaryTermApproved e)
        {

        }

        public void When(GlossaryTermRevised e)
        {
            var term = Dictionary.Single(x => x.Identifier == e.Identifier);
            term.Name = e.Name;
            term.Content = e.Content;
        }

        public void When(GlossaryTermRejected e)
        {
            var term = Dictionary.Single(x => x.Identifier == e.Term);
            Dictionary.Remove(term);
        }

        public void When(GlossaryTermLinked e)
        {
            var term = Dictionary.Single(x => x.Identifier == e.TermId);
            term.Contents.Add(new GlossaryItemContent 
            { 
                Identifier = e.RelationshipId,
                Container = e.ContainerId,
                ContentLabel = e.ContentLabel
            });
        }

        public void When(GlossaryTermUnlinked e)
        {
            var term = Dictionary.Single(x => x.Identifier == e.TermId);
            term.Contents.RemoveAll(c => c.Identifier == e.RelationshipId);
        }
    }
}
