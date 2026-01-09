using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Exceptions;

using InSite.Domain.Surveys.Sessions;

namespace InSite.Application.Surveys.Read
{
    /// <summary>
    /// Implements the process manager for Response events. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class ResponseChangeProjector
    {
        private readonly ISurveyStore _store;

        public ResponseChangeProjector(IChangeQueue publisher, ISurveyStore store)
        {
            _store = store;

            publisher.Subscribe<ResponseAnswerAdded>(Handle);
            publisher.Subscribe<ResponseAnswerChanged>(Handle);
            publisher.Subscribe<ResponseGroupChanged>(Handle);
            publisher.Subscribe<ResponsePeriodChanged>(Handle);
            publisher.Subscribe<ResponseUserChanged>(Handle);
            publisher.Subscribe<ResponseOptionSelected>(Handle);
            publisher.Subscribe<ResponseOptionUnselected>(Handle);
            publisher.Subscribe<ResponseOptionsAdded>(Handle);
            publisher.Subscribe<ResponseSessionCompleted>(Handle);
            publisher.Subscribe<ResponseSessionConfirmed>(Handle);
            publisher.Subscribe<ResponseSessionCreated>(Handle);
            publisher.Subscribe<ResponseSessionLocked>(Handle);
            publisher.Subscribe<ResponseSessionReviewed>(Handle);
            publisher.Subscribe<ResponseSessionStarted>(Handle);
            publisher.Subscribe<ResponseSessionUnlocked>(Handle);
            publisher.Subscribe<ResponseSessionDeleted>(Handle);
            publisher.Subscribe<ResponseSessionFormConsent>(Handle);
        }

        public void Handle(ResponseAnswerAdded e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseAnswerChanged e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseGroupChanged e)
            => _store.UpdateResponse(e);

        public void Handle(ResponsePeriodChanged e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseUserChanged e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseOptionSelected e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseOptionUnselected e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseOptionsAdded e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseSessionCompleted e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseSessionConfirmed e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseSessionCreated e)
            => _store.InsertResponse(e);

        public void Handle(ResponseSessionLocked e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseSessionReviewed e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseSessionStarted e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseSessionUnlocked e)
            => _store.UpdateResponse(e);

        public void Handle(ResponseSessionFormConsent e)
           => _store.UpdateResponse(e);


        public void Handle(ResponseSessionDeleted e)
            => _store.DeleteResponse(e);

        public void Replay(IChangeStore store, Action<string, int, int, Guid> progress, Guid? id)
        {
            // Clear all the existing data in the query store for this projection.
            if (id.HasValue)
                _store.DeleteAllResponses(id.Value);
            else
                _store.DeleteAllResponses();

            // Get the subset of events for which this projection is a subscriber. 
            var changes = store.GetChanges("Response", id, null);

            // Handle each of the events in the order they occurred.
            for (var i = 0; i < changes.Length; i++)
            {
                var e = changes[i];

                progress("Response", i + 1, changes.Length, e.AggregateIdentifier);

                var handler = GetType().GetMethod("Handle", new Type[] { e.GetType() });
                if (handler == null)
                    throw new MethodNotFoundException(GetType(), "Handle", e.GetType());

                handler.Invoke(this, new[] { e });
            }
        }
    }
}