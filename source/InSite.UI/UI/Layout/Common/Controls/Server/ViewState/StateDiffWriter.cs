using System;
using System.Collections;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class StateDiffWriter : IStateWriter
    {
        private const int FlagSize = 32;

        public int Count => _values.Length;

        private int _valueIndex;
        private int _bitIndex;
        private int _flagIndex;
        private uint _flag;

        private uint[] _flags;
        private object[] _values;
        private ArrayList _diff;

        public StateDiffWriter(object[] values)
        {
            _valueIndex = 0;
            _bitIndex = 0;
            _flagIndex = 0;
            _flag = 0;

            _flags = new uint[(int)Math.Ceiling((decimal)values.Length / FlagSize)];
            _values = values;
            _diff = new ArrayList(_values.Length);
        }

        public void Add<T>(T value) => Add(value, DefaultIsEqual);

        public void Add<T>(T value, Func<T, T, bool> isEqual)
        {
            if (_valueIndex >= _values.Length)
                throw ApplicationError.Create("Invalid values size");

            if (!isEqual(value, (T)_values[_valueIndex]))
            {
                _flag |= (uint)1 << _bitIndex;
                _diff.Add(value);
            }

            _valueIndex += 1;
            _bitIndex += 1;

            if (_bitIndex == FlagSize)
            {
                _bitIndex = 0;
                _flags[_flagIndex++] = _flag;
                _flag = 0;
            }
        }

        private static bool DefaultIsEqual<T>(T a, T b) => object.Equals(a, b);

        public object ToObject()
        {
            if (_valueIndex != _values.Length)
                throw ApplicationError.Create("Invalid state size");

            if (_diff.Count == 0)
                return null;

            if (_diff.Count == _values.Length)
                return _diff.ToArray();

            if (_bitIndex != 0)
                _flags[_flagIndex] = _flag;

            return new Triplet(_diff.ToArray(), _values.Length, _flags);
        }
    }
}