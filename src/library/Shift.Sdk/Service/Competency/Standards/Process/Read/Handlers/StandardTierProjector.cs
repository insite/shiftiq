using Shift.Common.Timeline.Changes;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Read
{
    public class StandardTierProjector
    {
        private readonly IStandardTierStore _store;

        public StandardTierProjector(IChangeQueue publisher, IStandardTierStore store)
        {
            _store = store;

            publisher.Subscribe<StandardCreated>(Handle);
            publisher.Subscribe<StandardRemoved>(Handle);
            publisher.Subscribe<StandardFieldGuidModified>(Handle);
            publisher.Subscribe<StandardFieldsModified>(Handle);
        }

        public void Handle(StandardCreated e) => _store.Insert(e);

        public void Handle(StandardRemoved e) => _store.Delete(e);

        public void Handle(StandardFieldGuidModified e) => _store.Update(e);

        public void Handle(StandardFieldsModified e) => _store.Update(e);
    }
}
