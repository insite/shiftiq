using Shift.Common.Timeline.Changes;

using InSite.Domain.Contacts;

namespace InSite.Application.Contacts.Read
{
    /// <summary>
    /// Implements the projector for Person Secret changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A 
    /// processor, in contrast, should *never* replay past changes.
    /// </remarks>
    public class PersonSecretChangeProjector
    {
        private readonly IPersonSecretStore _store;

        public PersonSecretChangeProjector(IChangeQueue publisher, IPersonSecretStore store)
        {
            _store = store;

            publisher.Subscribe<PersonSecretAdded>(Handle);
            publisher.Subscribe<PersonSecretRemoved>(Handle);
        }

        public void Handle(PersonSecretAdded e)
            => _store.Insert(e);

        public void Handle(PersonSecretRemoved e)
            => _store.Delete(e);
    }
}
