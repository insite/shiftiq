using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Glossaries.Read;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class GlossarySearch : IGlossarySearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public int CountTerms(GlossaryTermFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public QGlossaryTerm GetTerm(Guid term)
        {
            using (var db = CreateContext())
                return db.GlossaryTerms
                    .AsNoTracking()
                    .FirstOrDefault(x => x.TermIdentifier == term);
        }

        public QGlossaryTerm GetTerm(Guid glossary, string term)
        {
            using (var db = CreateContext())
                return db.GlossaryTerms
                    .AsNoTracking()
                    .FirstOrDefault(x => x.GlossaryIdentifier == glossary && x.TermName == term);
        }

        public List<QGlossaryTerm> GetTerms(IEnumerable<Guid> ids)
        {
            using (var db = CreateContext())
            {
                return db.GlossaryTerms
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.TermIdentifier))
                    .ToList();
            }
        }

        public List<QGlossaryTerm> GetTerms(GlossaryTermFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db);

                if (filter.OrderBy.IsNotEmpty())
                    query = query.OrderBy(filter.OrderBy);
                else
                    query = query.OrderBy(x => x.TermName);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private IQueryable<QGlossaryTerm> CreateQuery(GlossaryTermFilter filter, InternalDbContext db)
        {
            var query = db.GlossaryTerms
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (filter.GlossaryIdentifier.HasValue)
                query = query.Where(x => x.GlossaryIdentifier == filter.GlossaryIdentifier.Value);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.TermIdentifier.IsNotEmpty())
            {
                if (Guid.TryParse(filter.TermIdentifier, out var termId))
                    query = query.Where(x => x.TermIdentifier == termId);
                else
                    query = query.Where(x => x.TermIdentifier.ToString().Contains(filter.TermIdentifier));
            }

            if (filter.TermName.IsNotEmpty())
                query = query.Where(x => x.TermName.StartsWith(filter.TermName));

            if (filter.TermTitle.HasValue())
                query = query.Where(
                    x => CoreFunctions.IsContentContains(x.TermIdentifier, "Title", null, DbFunctions.AsUnicode(filter.TermTitle)));

            if (filter.TermDefinition.IsNotEmpty())
                query = query.Where(x => CoreFunctions.IsContentContains(x.TermIdentifier, "Description", null, filter.TermDefinition));

            if (filter.TermKeyword.HasValue())
                query = query.Where(
                    x => x.TermName.Contains(DbFunctions.AsUnicode(filter.TermKeyword))
                      || CoreFunctions.IsContentContains(x.TermIdentifier, "Title", null, DbFunctions.AsUnicode(filter.TermKeyword)));

            if (filter.TermStatus.IsNotEmpty())
                query = query.Where(x => x.TermStatus == filter.TermStatus);

            if (filter.IsTranslated.HasValue)
            {
                if (filter.IsTranslated.Value)
                    query = query.Where(x => CoreFunctions.IsContentTranslated(x.TermIdentifier, "Description"));
                else
                    query = query.Where(x => !CoreFunctions.IsContentTranslated(x.TermIdentifier, "Description"));
            }

            if (filter.ExcludeTermIdentifiers.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeTermIdentifiers.Contains(x.TermIdentifier));

            if (filter.RevisionCountFrom.HasValue)
                query = query.Where(x => x.RevisionCount >= filter.RevisionCountFrom.Value);

            if (filter.RevisionCountThru.HasValue)
                query = query.Where(x => x.RevisionCount <= filter.RevisionCountThru.Value);

            if (filter.ProposedBy.IsNotEmpty())
                query = query.Where(x => x.ProposedBy.Contains(filter.ProposedBy));

            if (filter.ProposedSince.HasValue)
                query = query.Where(x => x.Proposed >= filter.ProposedSince.Value);

            if (filter.ProposedBefore.HasValue)
                query = query.Where(x => x.Proposed < filter.ProposedBefore.Value);

            if (filter.ApprovedBy.IsNotEmpty())
                query = query.Where(x => x.ApprovedBy.Contains(filter.ApprovedBy));

            if (filter.ApprovedSince.HasValue)
                query = query.Where(x => x.Approved >= filter.ApprovedSince.Value);

            if (filter.ApprovedBefore.HasValue)
                query = query.Where(x => x.Approved < filter.ApprovedBefore.Value);

            if (filter.LastRevisedBy.IsNotEmpty())
                query = query.Where(x => x.LastRevisedBy.Contains(filter.LastRevisedBy));

            if (filter.LastRevisedSince.HasValue)
                query = query.Where(x => x.LastRevised >= filter.LastRevisedSince.Value);

            if (filter.LastRevisedBefore.HasValue)
                query = query.Where(x => x.LastRevised < filter.LastRevisedBefore.Value);

            return query;
        }

        public List<QGlossaryTerm> GetContainerTerms(Guid glossary, Guid container, string label = null)
        {
            using (var db = CreateContext())
            {
                return db.GlossaryTermContents
                    .Where(x => x.Term.GlossaryIdentifier == glossary
                             && x.ContainerIdentifier == container
                             && x.ContentLabel == label)
                    .Select(x => x.Term)
                    .ToList();
            }
        }

        public int CountTermContents(GlossaryTermContentFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<QGlossaryTermContent> GetTermContents(GlossaryTermContentFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .Include(x => x.Term)
                    .OrderBy(x => x.Term.TermName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<QGlossaryTermContent> CreateQuery(GlossaryTermContentFilter filter, InternalDbContext db)
        {
            var query = db.GlossaryTermContents.AsNoTracking().AsQueryable();

            if (filter == null)
                return query;

            if (filter.ContainerIdentifier.HasValue)
                query = query.Where(x => x.ContainerIdentifier == filter.ContainerIdentifier);

            if (filter.TermIdentifier.HasValue)
                query = query.Where(x => x.TermIdentifier == filter.TermIdentifier);

            if (filter.GlossaryIdentifier.HasValue)
                query = query.Where(x => x.Term.GlossaryIdentifier == filter.GlossaryIdentifier.Value);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Term.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.ContentLabel.IsNotEmpty())
                query = query.Where(x => x.ContentLabel == filter.ContentLabel);

            if (filter.TermKeyword.HasValue())
                query = query.Where(
                    x => x.Term.TermName.StartsWith(DbFunctions.AsUnicode(filter.TermKeyword))
                      || CoreFunctions.IsContentContains(x.TermIdentifier, null, null, DbFunctions.AsUnicode(filter.TermKeyword)));

            return query;
        }
    }
}
