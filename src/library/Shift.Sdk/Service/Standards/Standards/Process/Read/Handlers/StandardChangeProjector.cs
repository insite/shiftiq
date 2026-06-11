using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Application.Contents.Read;
using InSite.Domain.Standards;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Standards.Read
{
    /// <summary>
    /// Implements the projector for Standard changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A 
    /// processor, in contrast, should *never* replay past changes.
    /// </remarks>
    public class StandardChangeProjector
    {
        private readonly IStandardStore _standardStore;
        private readonly IStandardTierStore _tierStore;
        private readonly IContentStore _contentStore;

        public StandardChangeProjector(IChangeQueue publisher, IStandardStore standardStore, IStandardTierStore tierStore, IContentStore contentStore)
        {
            _standardStore = standardStore;
            _tierStore = tierStore;
            _contentStore = contentStore;

            publisher.Subscribe<StandardCreated>(Handle);
            publisher.Subscribe<StandardRemoved>(Handle);
            publisher.Subscribe<StandardTimestampsModified>(Handle);
            publisher.Subscribe<StandardCategoryAdded>(Handle);
            publisher.Subscribe<StandardCategoryRemoved>(Handle);
            publisher.Subscribe<StandardConnectionAdded>(Handle);
            publisher.Subscribe<StandardConnectionRemoved>(Handle);
            publisher.Subscribe<StandardContainmentAdded>(Handle);
            publisher.Subscribe<StandardContainmentModified>(Handle);
            publisher.Subscribe<StandardContainmentRemoved>(Handle);
            publisher.Subscribe<StandardContentModified>(Handle);
            publisher.Subscribe<StandardOrganizationAdded>(Handle);
            publisher.Subscribe<StandardOrganizationRemoved>(Handle);
            publisher.Subscribe<StandardAchievementAdded>(Handle);
            publisher.Subscribe<StandardAchievementRemoved>(Handle);
            publisher.Subscribe<StandardGroupAdded>(Handle);
            publisher.Subscribe<StandardGroupRemoved>(Handle);
            publisher.Subscribe<StandardFieldTextModified>(Handle);
            publisher.Subscribe<StandardFieldDateOffsetModified>(Handle);
            publisher.Subscribe<StandardFieldBoolModified>(Handle);
            publisher.Subscribe<StandardFieldIntModified>(Handle);
            publisher.Subscribe<StandardFieldDecimalModified>(Handle);
            publisher.Subscribe<StandardFieldGuidModified>(Handle);
            publisher.Subscribe<StandardFieldsModified>(Handle);
        }

        public void Handle(StandardCreated e)
        {
            SaveStandardContent(e.Content, e);

            _standardStore.Insert(e);
        }

        public void Handle(StandardRemoved e) => _standardStore.Delete(e);

        public void Handle(StandardTimestampsModified e) => _standardStore.Update(e);

        public void Handle(StandardCategoryAdded e) => _standardStore.Update(e);

        public void Handle(StandardCategoryRemoved e) => _standardStore.Update(e);

        public void Handle(StandardConnectionAdded e) => _standardStore.Update(e);

        public void Handle(StandardConnectionRemoved e) => _standardStore.Update(e);

        public void Handle(StandardContainmentAdded e) => _standardStore.Update(e);

        public void Handle(StandardContainmentModified e) => _standardStore.Update(e);

        public void Handle(StandardContainmentRemoved e) => _standardStore.Update(e);

        public void Handle(StandardContentModified e)
        {
            SaveStandardContent(e.Content, e);

            _standardStore.Update(e);
        }

        public void Handle(StandardOrganizationAdded e) => _standardStore.Update(e);

        public void Handle(StandardOrganizationRemoved e) => _standardStore.Update(e);

        public void Handle(StandardAchievementAdded e) => _standardStore.Update(e);

        public void Handle(StandardAchievementRemoved e) => _standardStore.Update(e);

        public void Handle(StandardGroupAdded e) => _standardStore.Update(e);

        public void Handle(StandardGroupRemoved e) => _standardStore.Update(e);

        public void Handle(StandardFieldTextModified e) => _standardStore.Update(e);

        public void Handle(StandardFieldDateOffsetModified e) => _standardStore.Update(e);

        public void Handle(StandardFieldBoolModified e) => _standardStore.Update(e);

        public void Handle(StandardFieldIntModified e) => _standardStore.Update(e);

        public void Handle(StandardFieldDecimalModified e) => _standardStore.Update(e);

        public void Handle(StandardFieldGuidModified e) => _standardStore.Update(e);

        public void Handle(StandardFieldsModified e) => _standardStore.Update(e);

        /// <summary>
        /// Regenerate the projection of standard changes from the log to query tables.
        /// </summary>
        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear existing data from the query tables.
            if (id.HasValue)
            {
                _standardStore.DeleteAll(id.Value);
                _tierStore.DeleteAll(id.Value);
            }
            else
            {
                _standardStore.DeleteAll();
                _tierStore.DeleteAll();
            }

            // Get the subset of changes for which this projector is a subscriber. 
            var changes = store.GetChanges("Standard", id, null);

            // Handle each of the changes in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Standard", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }

        private void SaveStandardContent(ContentContainer content, IChange change)
        {
            _contentStore.SaveContainer(change.OriginOrganization, ContentContainerType.Standard, change.AggregateIdentifier, content);
        }
    }
}
