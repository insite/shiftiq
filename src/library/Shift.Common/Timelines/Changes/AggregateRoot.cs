using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Shift.Common.Timeline.Commands;
using Shift.Common.Timeline.Exceptions;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Implements the base class for all aggregate roots. An aggregate forms a tree or graph of object relations. The 
    /// aggregate root is the top-level container, which speaks for the whole and may delegates down to the rest. It is 
    /// important because it is the one that the rest of the world communicates with.
    /// </summary>
    public abstract class AggregateRoot
    {
        /// <summary>
        /// Changes to the state of the aggregate that are not yet committed to a persistent event store.
        /// </summary>
        private readonly List<IChange> _changes = new List<IChange>();

        /// <summary>
        /// Represents the state (i.e. data/packet) for the aggregate.
        /// </summary>
        public AggregateState State { get; set; }

        /// <summary>
        /// Uniquely identifies the aggregate.
        /// </summary>
        public Guid AggregateIdentifier { get; set; }

        /// <summary>
        /// Uniquely identifies the aggregate.
        /// </summary>
        public Guid RootAggregateIdentifier { get; set; }

        /// <summary>
        /// Current version of the aggregate.
        /// </summary>
        public int AggregateVersion { get; set; }

        /// <summary>
        /// Every aggregate must override this method to create the object that holds its current state.
        /// </summary>
        public abstract AggregateState CreateState();

        /// <summary>
        /// Returns all uncommitted changes. 
        /// </summary>
        /// <returns></returns>
        public IChange[] GetUncommittedChanges()
        {
            IChange[] changes = null;

            LockAndRun(() =>
            {
                changes = _changes.ToArray();
            });

            return changes;
        }

        /// <summary>
        /// Returns all uncommitted changes and clears them from the aggregate.
        /// </summary>
        public IChange[] FlushUncommittedChanges()
        {
            IChange[] changes = null;

            LockAndRun(() =>
            {
                changes = _changes.ToArray();

                var i = 0;

                foreach (var change in changes)
                {
                    if (change.AggregateIdentifier == Guid.Empty && AggregateIdentifier == Guid.Empty)
                        throw new MissingAggregateIdentifierException(GetType(), change.GetType());

                    if (change.AggregateIdentifier == Guid.Empty)
                        change.AggregateIdentifier = AggregateIdentifier;

                    i++;

                    change.AggregateVersion = AggregateVersion + i;
                }

                AggregateVersion += changes.Length;

                _changes.Clear();
            });

            return changes;
        }

        /// <summary>
        /// Assigns a specific organization and user identity to every uncomitted change.
        /// </summary>
        public void Identify(Guid organization, Guid user)
        {
            LockAndRun(() =>
            {
                foreach (var change in _changes)
                    change.Identify(organization, user);
            });
        }

        /// <summary>
        /// Loads an aggregate from a stream of events.
        /// </summary>
        public void Rehydrate(IEnumerable<IChange> history)
        {
            LockAndRun(() =>
            {
                foreach (var change in history.ToArray())
                {
                    if (change.AggregateVersion != AggregateVersion + 1)
                        throw new UnorderedChangesException(change.AggregateIdentifier);

                    ApplyChange(change);

                    AggregateIdentifier = change.AggregateIdentifier;
                    AggregateVersion++;
                }
            });
        }

        /// <summary>
        /// Applies a change to the aggregate state AND appends the event to the history of uncommited changes.
        /// </summary>
        public void Apply(IChange change)
        {
            if (change.AggregateIdentifier == Guid.Empty)
                change.AggregateIdentifier = AggregateIdentifier;

            LockAndRun(() =>
            {
                ApplyChange(change);
                _changes.Add(change);
            });
        }

        /// <summary>
        /// Applies a change to the aggregate state. This method is called internally when rehydrating an aggregate, 
        /// and you can override this when custom handling is needed.
        /// </summary>
        virtual protected void ApplyChange(IChange change)
        {
            if (State == null)
                State = CreateState();

            if (_context != null)
                change.Identify(_context.Organization, _context.User);

            State.Apply(change);
        }

        private AggregateRunContext _context = null;

        public void LockAndRun(Command context, Action action)
        {
            LockAndRun(action, new AggregateRunContext(context.OriginOrganization, context.OriginUser));
        }

        public void LockAndRun(Action action)
        {
            LockAndRun(action, null);
        }

        private void LockAndRun(Action action, AggregateRunContext context)
        {
            // LockEnter
            {
                const int LockTimeoutInMs = 2 * 1000; // 2 sec
                const int LockWaitInMs = 100;
                const int LockTryCount = 50;

                var tryCount = LockTryCount;

                while (!Monitor.TryEnter(this, LockTimeoutInMs))
                {
                    if (--tryCount <= 0)
                        throw new ConcurrencyException($"The aggregate {AggregateIdentifier} cannot be locked by the current thread.");

                    Thread.Sleep(LockWaitInMs);
                }
            }

            try
            {
                if (context != null)
                {
                    if (_context == null)
                        _context = context;
                    else if (_context != context)
                        throw new AggregateException("Context already assigned to this aggregate");
                }

                action?.Invoke();
            }
            finally
            {
                _context = null;

                // LockExit
                {
                    Monitor.Exit(this);
                }
            }
        }
    }
}
