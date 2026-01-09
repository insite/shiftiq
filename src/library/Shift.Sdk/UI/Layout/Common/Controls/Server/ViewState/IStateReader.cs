using System;

namespace Shift.Sdk.UI
{
    public interface IStateReader
    {
        int Count { get; }
        bool Get<T>(Action<T> set);
        void Validate();
    }
}
