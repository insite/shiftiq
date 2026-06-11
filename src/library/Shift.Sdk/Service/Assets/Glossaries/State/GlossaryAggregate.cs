using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Glossaries
{
    public class GlossaryAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new Glossary();

        public Glossary Data => (Glossary)State;

        #region Methods (commands)

        public void Initialize()
        {
            if (Data != null)
                return;

            var change = new GlossaryInitialized2();

            Apply(change);
        }

        public void ProposeGlossaryTerm(Guid id, string name, ContentContainer content)
        {
            // Validate the parameters.

            if (Data.Dictionary.Any(x => x.Identifier == id || x.Name == name))
                throw new DuplicateGlossaryTermException(AggregateIdentifier, name);

            // Create the event.

            var change = new GlossaryTermProposed(id, name, content);

            // Apply the state chage.

            Apply(change);
        }

        public void ApproveGlossaryTerm(Guid id)
        {
            // Validate the parameters.

            if (!Data.Dictionary.Any(x => x.Identifier == id))
                return;

            // Create the event.

            var change = new GlossaryTermApproved(id);

            // Apply the state chage.

            Apply(change);
        }

        public void ReviseGlossaryTerm(Guid id, string name, ContentContainer content)
        {
            if (!Data.Dictionary.Any(x => x.Identifier == id))
                return;

            var change = new GlossaryTermRevised(id, name, content);

            Apply(change);
        }

        public void LinkGlossaryTerm(Guid relationship, Guid term, Guid containerId, string containerType, string contentLabel)
        {
            var termItem = Data.Dictionary.FirstOrDefault(x => x.Identifier == term);
            if (termItem == null)
                return;

            if (termItem.Contents.Any(x => x.Container == containerId && string.Equals(x.ContentLabel, contentLabel, StringComparison.OrdinalIgnoreCase)))
                return;

            var change = new GlossaryTermLinked(relationship, term, containerId, containerType, contentLabel);

            Apply(change);
        }

        public void UnlinkGlossaryTerm(Guid relationship, Guid term)
        {
            var termItem = Data.Dictionary.FirstOrDefault(x => x.Identifier == term);
            if (termItem == null)
                return;

            if (!termItem.Contents.Any(x => x.Identifier == relationship))
                return;

            var change = new GlossaryTermUnlinked(relationship, term);

            Apply(change);
        }

        public void RejectGlossaryTerm(Guid id)
        {
            if (!Data.Dictionary.Any(x => x.Identifier == id))
                return;

            var change = new GlossaryTermRejected(id);

            Apply(change);
        }

        #endregion
    }
}
