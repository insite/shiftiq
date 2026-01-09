using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Exceptions;
using Shift.Common.Timeline.Services;

using Shift.Common;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Implements a basic event queue.
    /// </summary>
    public class ChangeQueue : IChangeQueue
    {
        private readonly IJsonSerializer _serializer;

        /// <summary>
        /// An event's full class name is used as the key to a list of event-handling methods.
        /// </summary>
        private readonly Dictionary<string, List<Action<IChange>>> _subscribers;

        /// <summary>
        /// In a multi-organization system we may want to allow each individual organization to extend the handling of 
        /// an event. The class name and the organization identifier is used as the unique key here.
        /// </summary>
        private readonly Dictionary<(string, Guid), Action<IChange>> _precursors, _extenders;

        /// <summary>
        /// In a multi-organization system we may want to allow each individual organization to override/customize the handling of 
        /// an event. The class name and the organization identifier is used as the unique key here.
        /// </summary>
        private readonly Dictionary<(string, Guid), Action<IChange>> _overriders;

        /// <summary>
        /// Constructs the queue.
        /// </summary>
        public ChangeQueue()
        {
            _subscribers = new Dictionary<string, List<Action<IChange>>>();
            _precursors = new Dictionary<(string, Guid), Action<IChange>>();
            _extenders = new Dictionary<(string, Guid), Action<IChange>>();
            _overriders = new Dictionary<(string, Guid), Action<IChange>>();

            _serializer = ServiceLocator.Instance.GetService<IJsonSerializer>();
        }

        /// <summary>
        /// Invokes each subscriber method registered to handle the event.
        /// </summary>
        /// <param name="change"></param>
        public void Publish(IChange change)
        {
            var name = _serializer.GetClassName(change.GetType());

            var precursorExists = _precursors.ContainsKey((name, change.OriginOrganization));
            var subscriberExists = _subscribers.ContainsKey(name);
            var extenderExists = _extenders.ContainsKey((name, change.OriginOrganization));

            if (_overriders.ContainsKey((name, change.OriginOrganization)))
            {
                var overrider = _overriders[(name, change.OriginOrganization)];
                overrider?.Invoke(change);
            }
            else if (precursorExists || subscriberExists || extenderExists)
            {
                if (precursorExists)
                {
                    var precursor = _precursors[(name, change.OriginOrganization)];
                    precursor?.Invoke(change);
                }

                if (subscriberExists)
                {
                    var actions = _subscribers[name];
                    foreach (var action in actions)
                        action.Invoke(change);
                }

                if (extenderExists)
                {
                    var extender = _extenders[(name, change.OriginOrganization)];
                    extender?.Invoke(change);
                }
            }
            else
            {
                throw new UnhandledChangeException(name);
            }
        }

        /// <summary>
        /// Any number of subscribers can register for an event, and any one subscriber can register any number of
        /// methods to be invoked when the event is published. 
        /// </summary>
        public void Subscribe<T>(Action<T> action) where T : IChange
        {
            var name = _serializer.GetClassName(typeof(T));

            if (!_subscribers.Any(x => x.Key == name))
                _subscribers.Add(name, new List<Action<IChange>>());

            _subscribers[name].Add((change) => action((T)change));
        }

        /// <summary>
        /// Register a custom organization-specific handler for the event.
        /// </summary>
        public void Extend<T>(Action<T> action, Guid organization, bool before = false) where T : IChange
        {
            var name = _serializer.GetClassName(typeof(T));

            if (before)
            {
                if (_precursors.Any(x => x.Key.Item1 == name && x.Key.Item2 == organization))
                    throw new AmbiguousChangeHandlerException(name);
                _precursors.Add((name, organization), (command) => action((T)command));
            }
            else
            {
                if (_extenders.Any(x => x.Key.Item1 == name && x.Key.Item2 == organization))
                    throw new AmbiguousChangeHandlerException(name);
                _extenders.Add((name, organization), (command) => action((T)command));
            }
        }

        public bool Extends<T>(Guid organization)
            => _extenders.Any(x => x.Key.Item1 == _serializer.GetClassName(typeof(T)) && x.Key.Item2 == organization);

        /// <summary>
        /// Register a custom organization-specific handler for the event.
        /// </summary>
        public void Override<T>(Action<T> action, Guid organization) where T : IChange
        {
            var name = _serializer.GetClassName(typeof(T));

            if (_overriders.Any(x => x.Key.Item1 == name && x.Key.Item2 == organization))
                throw new AmbiguousCommandHandlerException(name);

            _overriders.Add((name, organization), (command) => action((T)command));
        }
    }
}
