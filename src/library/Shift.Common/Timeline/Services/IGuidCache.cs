using System;

namespace Shift.Common.Timeline.Services
{
    /// <summary>
    /// Provides a generic cache for objects indexed by globally unique identifiers.
    /// </summary>
    public interface IGuidCache<T>
    {
        void Add(Guid id, T value, int timeout, bool restartTimer = false);
        
        T Get(Guid id);

        void Remove(Guid id);
    }
}