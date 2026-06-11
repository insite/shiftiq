using System;

namespace Shift.Common.Timeline.Services
{
    /// <summary>
    /// Generates globally unique identifiers.
    /// </summary>
    public interface IGuidGenerator
    {
        Guid NewGuid(Type t);
        Guid NewGuid();
    }
}