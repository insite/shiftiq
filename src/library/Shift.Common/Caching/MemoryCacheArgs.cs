using System;

namespace Shift.Common
{
    public class MemoryCacheArgs<TKey, TData> : EventArgs
    {
        public TKey Key { get; }
        public TData Data { get; }

        public MemoryCacheArgs(TKey key, TData data)
        {
            Key = key;
            Data = data;
        }
    }

    public delegate void MemoryCacheHandler<TKey, TData>(object sender, MemoryCacheArgs<TKey, TData> args);
}
