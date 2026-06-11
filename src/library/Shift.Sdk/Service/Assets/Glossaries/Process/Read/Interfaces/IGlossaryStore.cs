using System;

using Shift.Common;
namespace InSite.Application.Glossaries.Read
{
    public interface IGlossaryStore
    {
        void InitializeGlossary(Guid glossary, Guid organization);
        void ProposeTerm(Guid glossary, Guid organization, Guid term, string name, ContentContainer content, DateTimeOffset when, Guid who);
        void ApproveTerm(Guid identifier, DateTimeOffset when, Guid who);
        void ReviseTerm(Guid identifier, string name, ContentContainer content, DateTimeOffset when, Guid who);
        void DeleteTerm(Guid identifier);
        void LinkTerm(Guid relationshipId, Guid termId, Guid containerId, string containerType, string contentLabel);
        void UnlinkTerm(Guid relationshipId);
    }
}