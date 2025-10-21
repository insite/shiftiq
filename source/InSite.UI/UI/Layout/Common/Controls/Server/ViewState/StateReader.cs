using System;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class StateReader : IStateReader
    {
        private const int FlagSize = 32;

        public int Count => _length;

        private int _diffIndex;
        private int _itemIndex;
        private int _bitIndex;
        private int _flagIndex;
        private uint _flag;

        private int _length;
        private uint[] _flags;
        private object[] _values;

        public StateReader(object state)
        {
            _diffIndex = 0;
            _itemIndex = 0;
            _bitIndex = FlagSize;
            _flagIndex = 0;

            if (state is Triplet triplet)
            {
                _values = (object[])triplet.First;
                _length = (int)triplet.Second;
                _flags = (uint[])triplet.Third;
            }
            else
            {
                _values = (object[])state;
                _length = _values.Length;
                _flags = null;
            }
        }

        public bool Get<T>(Action<T> set)
        {
            if (_itemIndex == _length)
                throw ApplicationError.Create("Item index is out of range");

            if (_bitIndex == FlagSize)
            {
                _bitIndex = 0;
                _flag = _flags == null
                    ? uint.MaxValue
                    : _flags[_flagIndex++];
            }
            else
            {
                _flag = _flag >> 1;
            }

            var hasValue = (_flag & 1) == 1;

            if (hasValue)
                set((T)_values[_diffIndex++]);

            _itemIndex++;
            _bitIndex++;

            return hasValue;
        }

        public void Validate()
        {
            if (_itemIndex != _length)
                throw ApplicationError.Create("Invalid state size");
        }
    }
}