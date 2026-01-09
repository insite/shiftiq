using System;
using System.Collections.Generic;

namespace InSite.Application.Glossaries.Read
{
    public interface IGlossarySearch
    {
        int CountTerms(GlossaryTermFilter filter);
        QGlossaryTerm GetTerm(Guid term);
        QGlossaryTerm GetTerm(Guid glossary, string term);
        List<QGlossaryTerm> GetTerms(IEnumerable<Guid> ids);
        List<QGlossaryTerm> GetTerms(GlossaryTermFilter filter);
        List<QGlossaryTerm> GetContainerTerms(Guid glossary, Guid container, string label = null);
        int CountTermContents(GlossaryTermContentFilter filter);
        List<QGlossaryTermContent> GetTermContents(GlossaryTermContentFilter filter);
    }
}
