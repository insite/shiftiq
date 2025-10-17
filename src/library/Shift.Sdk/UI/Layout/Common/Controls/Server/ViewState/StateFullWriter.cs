using System;
using System.Collections;

namespace Shift.Sdk.UI
{
    public class StateFullWriter : IStateWriter
    {
        public int Count => _values.Count;

        private ArrayList _values;

        public StateFullWriter(int capacity = 4)
        {
            _values = new ArrayList(capacity);
        }

        public void Add<T>(T value) => Add(value, null);

        public void Add<T>(T value, Func<T, T, bool> isEqual)
        {
            _values.Add(value);
        }

        public object[] ToArray()
        {
            return _values.ToArray();
        }

        object IStateWriter.ToObject() => ToArray();
    }
}