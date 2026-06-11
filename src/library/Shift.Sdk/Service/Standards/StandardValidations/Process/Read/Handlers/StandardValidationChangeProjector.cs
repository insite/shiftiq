using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Read
{
    /// <summary>
    /// Implements the projector for StandardValidation changes.
    /// </summary>
    /// <remarks>
    /// A projector is responsible for creating projections based on events. Changes can (and often should) be replayed
    /// by a projector, and there should be no side effects (aside from modifications to the projection tables). A 
    /// processor, in contrast, should *never* replay past changes.
    /// </remarks>
    public class StandardValidationChangeProjector
    {
        private readonly IStandardValidationStore _store;

        public StandardValidationChangeProjector(IChangeQueue publisher, IStandardValidationStore store)
        {
            _store = store;

            publisher.Subscribe<StandardValidationCreated>(Handle);
            publisher.Subscribe<StandardValidationRemoved>(Handle);
            publisher.Subscribe<StandardValidationTimestampsModified>(Handle);
            publisher.Subscribe<StandardValidationFieldBoolModified>(Handle);
            publisher.Subscribe<StandardValidationFieldDateOffsetModified>(Handle);
            publisher.Subscribe<StandardValidationFieldGuidModified>(Handle);
            publisher.Subscribe<StandardValidationFieldTextModified>(Handle);
            publisher.Subscribe<StandardValidationFieldsModified>(Handle);
            publisher.Subscribe<StandardValidationSelfValidated>(Handle);
            publisher.Subscribe<StandardValidationStatusModified>(Handle);
            publisher.Subscribe<StandardValidationSubmittedForValidation>(Handle);
            publisher.Subscribe<StandardValidationValidated>(Handle);
            publisher.Subscribe<StandardValidationExpired>(Handle);
            publisher.Subscribe<StandardValidationNotified>(Handle);
            publisher.Subscribe<StandardValidationLogAdded>(Handle);
            publisher.Subscribe<StandardValidationLogModified>(Handle);
            publisher.Subscribe<StandardValidationLogRemoved>(Handle);
        }

        public void Handle(StandardValidationCreated e) => _store.Insert(e);

        public void Handle(StandardValidationRemoved e) => _store.Delete(e);

        public void Handle(StandardValidationTimestampsModified e) => _store.Update(e);

        public void Handle(StandardValidationFieldTextModified e) => _store.Update(e);

        public void Handle(StandardValidationFieldDateOffsetModified e) => _store.Update(e);

        public void Handle(StandardValidationFieldBoolModified e) => _store.Update(e);

        public void Handle(StandardValidationFieldGuidModified e) => _store.Update(e);

        public void Handle(StandardValidationFieldsModified e) => _store.Update(e);

        public void Handle(StandardValidationSelfValidated e) => _store.Update(e);

        public void Handle(StandardValidationStatusModified e) => _store.Update(e);

        public void Handle(StandardValidationSubmittedForValidation e) => _store.Update(e);

        public void Handle(StandardValidationValidated e) => _store.Update(e);

        public void Handle(StandardValidationExpired e) => _store.Update(e);

        public void Handle(StandardValidationNotified e) => _store.Update(e);

        public void Handle(StandardValidationLogAdded e) => _store.Update(e);

        public void Handle(StandardValidationLogModified e) => _store.Update(e);

        public void Handle(StandardValidationLogRemoved e) => _store.Update(e);

        /// <summary>
        /// Regenerate the projection of standard validation changes from the log to query tables.
        /// </summary>
        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear existing data from the query tables.
            if (id.HasValue)
                _store.DeleteAll(id.Value);
            else
                _store.DeleteAll();

            // Get the subset of changes for which this projector is a subscriber. 
            var changes = store.GetChanges("StandardValidation", id, null);

            // Handle each of the changes in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Standard Validation", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}
