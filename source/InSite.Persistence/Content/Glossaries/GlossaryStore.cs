using System;
using System.Linq;

using InSite.Application.Glossaries.Read;

using Shift.Constant;

namespace InSite.Persistence
{
    public class GlossaryStore : IGlossaryStore
    {
        private readonly TContentStore ContentStore;

        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public GlossaryStore()
        {
            ContentStore = new TContentStore();
        }

        #region Methods (glossary)

        public void InitializeGlossary(Guid glossary, Guid organization)
        {
            OrganizationStore.SetupGlossary(glossary, organization);
        }

        #endregion

        #region Methods (glossary terms)

        public void ProposeTerm(Guid glossary, Guid organization, Guid term, string name, Shift.Common.ContentContainer content, DateTimeOffset when, Guid who)
        {
            using (var db = CreateContext())
            {
                var entity = new QGlossaryTerm
                {
                    GlossaryIdentifier = glossary,
                    OrganizationIdentifier = organization,
                    TermIdentifier = term,
                    TermName = name,
                    TermStatus = "Proposed",
                    Proposed = when,
                    ProposedBy = GetPersonName(who)
                };
                db.GlossaryTerms.Add(entity);
                db.SaveChanges();
            }

            ContentStore.SaveContainer(organization, ContentContainerType.GlossaryTerm, term, content);
        }

        public void ApproveTerm(Guid identifier, DateTimeOffset when, Guid who)
        {
            using (var db = CreateContext())
            {
                var entity = db.GlossaryTerms.Single(x => x.TermIdentifier == identifier);

                entity.TermStatus = "Approved";
                entity.Approved = when;
                entity.ApprovedBy = GetPersonName(who);

                db.SaveChanges();
            }
        }

        public void ReviseTerm(Guid identifier, string name, Shift.Common.ContentContainer content, DateTimeOffset when, Guid who)
        {
            QGlossaryTerm entity;

            using (var db = CreateContext())
            {
                entity = db.GlossaryTerms.Single(x => x.TermIdentifier == identifier);

                if (!string.IsNullOrEmpty(name))
                    entity.TermName = name;

                entity.TermStatus = "Revised";
                entity.LastRevised = when;
                entity.LastRevisedBy = GetPersonName(who);
                entity.RevisionCount++;

                db.SaveChanges();
            }

            if (content != null || !content.IsEmpty)
                ContentStore.SaveContainer(entity.OrganizationIdentifier, ContentContainerType.GlossaryTerm, entity.TermIdentifier, content);
        }

        public void DeleteTerm(Guid identifier)
        {
            using (var db = CreateContext())
            {
                var entity = db.GlossaryTerms.Single(x => x.TermIdentifier == identifier);
                db.GlossaryTerms.Remove(entity);
                db.SaveChanges();
            }
        }

        public void LinkTerm(Guid relationshipId, Guid termId, Guid containerId, string containerType, string contentLabel)
        {
            using (var db = CreateContext())
            {
                var entity = new QGlossaryTermContent
                {
                    RelationshipIdentifier = relationshipId,
                    TermIdentifier = termId,
                    ContainerType = containerType,
                    ContainerIdentifier = containerId,
                    ContentLabel = contentLabel,
                };
                db.GlossaryTermContents.Add(entity);
                db.SaveChanges();
            }
        }

        public void UnlinkTerm(Guid relationshipId)
        {
            using (var db = CreateContext())
            {
                var entity = db.GlossaryTermContents.Single(x => x.RelationshipIdentifier == relationshipId);
                db.GlossaryTermContents.Remove(entity);
                db.SaveChanges();
            }
        }

        #endregion

        #region Methods (contacts)

        private static string GetPersonName(Guid id)
        {
            var person = new ContactSearch().GetUser(id);

            return person == null ? UserNames.Someone : person.UserFullName;
        }

        #endregion
    }
}
