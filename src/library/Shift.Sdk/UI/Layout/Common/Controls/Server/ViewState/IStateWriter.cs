using System;

namespace Shift.Sdk.UI
{
    public interface IStateWriter
    {
        int Count { get; }
        void Add<T>(T value);
        void Add<T>(T value, Func<T, T, bool> equal);
        object ToObject();
    }
}
