using System;

using Shift.Common;

namespace Shift.Common
{
    /// <summary>
    /// An actor represents an individual person or group or system that performs actions through the UI and/or API.
    /// </summary>
    public class Actor : Model
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Secret { get; set; }

        public string Language { get; set; }
        public string TimeZone { get; set; }
    }

    /// <remarks>
    /// There are two distinct proxy use cases:
    ///   1. User acts on behalf of a Subject.
    ///   2. User is impersonated by an Agent.
    /// In scenario 1 it is understood the principal is a user who is authorized to act on behalf of
    /// In scenario 2 it is understood the principal is a user who is impersonated by another user.
    /// These two scenarios are mutually exclusive.
    /// </remarks>
    public class Proxy
    {
        public Actor Agent { get; set; }
        public Actor Subject { get; set; }

        public Proxy() { }

        public Proxy(Guid agent, Guid subject)
        {
            Agent = new Actor { Identifier = agent };
            Subject = new Actor { Identifier = subject };
        }

        public Proxy(Actor agent, Actor subject)
        {
            Agent = agent;
            Subject = subject;
        }
    }
}