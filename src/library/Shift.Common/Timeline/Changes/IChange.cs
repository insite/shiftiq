using System;

namespace Shift.Common.Timeline.Changes
{
    /// <summary>
    /// Defines the minimum set of properties that every event must implement.
    /// </summary>
    /// <remarks>
    /// An event represents something that has taken place in the domain. It is always named with a past-participle 
    /// verb, such as Order Confirmed. Since an event represents something in the past, it can be considered a 
    /// statement of fact, which can be used to make decisions in other parts of the system.Events are immutable 
    /// because they represent domain actions that occurred in the past, and the past cannot be altered.
    /// </remarks>
    public interface IChange
    {
        Guid AggregateIdentifier { get; set; }
        int AggregateVersion { get; set; }

        AggregateState AggregateState { get; set; }

        Guid OriginOrganization { get; set; }
        Guid OriginUser { get; set; }

        DateTimeOffset ChangeTime { get; set; }

        void Identify(Guid organization, Guid user);
    }
}
