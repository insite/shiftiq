using System;

namespace Shift.Common
{
    /// <remarks>
    /// The non-generic Cache class instantiates a Cache{object} that can be used with any type of 
    /// (mixed) content. It also publishes a static <c>.Global</c> member, so a cache can be used 
    /// without creating a dedicated instance. The <c>.Global</c> member is lazy-instantiated.
    /// </remarks>
    public class ObjectCache : MemoryCache<string, object>
    {
        private static readonly Lazy<ObjectCache> Lazy = new Lazy<ObjectCache>();

        /// <summary>
        /// Gets the global shared cache instance valid for the entire process.
        /// </summary>
        public static ObjectCache Global => Lazy.Value;
    }
}
