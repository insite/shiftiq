using System;

namespace Shift.Common
{
    public static class OAuthCacheService
    {
        private static MemoryCache<Guid, OAuthCacheEntry> Cache { get; set; } = new MemoryCache<Guid, OAuthCacheEntry>();

        public static Guid Add(OAuthCacheEntry entry)
        {
            var key = Guid.NewGuid();
            Cache.Add(key, entry, 5 * 50);
            return key;
        }

        public static void Remove(Guid Key)
        {
            Cache.Remove(Key);
        }

        public static OAuthCacheEntry Get(Guid Key)
        {
            if (Cache.TryGet(Key, out var entry)) return entry;
            return null;
        }
    }
}
