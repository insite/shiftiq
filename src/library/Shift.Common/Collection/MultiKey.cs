using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    [Serializable]
    public sealed class MultiKey : IEquatable<MultiKey>
    {
        #region Properties

        public object[] Values => _values;

        #endregion

        #region Fields

        private readonly object[] _values;
        private int _index;
        private int? _hash;

        #endregion

        #region Construction

        public MultiKey(int length)
        {
            _index = 0;
            _values = new object[length];
        }

        public MultiKey(params object[] values)
        {
            if (values.IsEmpty())
                throw new ArgumentNullException(nameof(values));

            _index = int.MaxValue;
            _values = values;
        }

        public MultiKey(IEnumerable<object> values)
        {
            if (values == null || !values.Any())
                throw new ArgumentNullException(nameof(values));

            _index = int.MaxValue;
            _values = values.ToArray();
        }

        #endregion

        #region Methods

        public void Add(object value)
        {
            _values[_index++] = value;
        }

        public override int GetHashCode()
        {
            return _hash ?? (int)(_hash = Values.Aggregate(17, (current, t) => current * 23 + t.GetHashCode()));
        }

        public override bool Equals(object obj) =>
            Equals(obj as MultiKey);

        public bool Equals(MultiKey key)
        {
            if (key == null || key.Values.Length != Values.Length)
                return false;

            for (var i = 0; i < Values.Length; i++)
                if (!Values[i].Equals(key.Values[i]))
                    return false;

            return true;
        }

        public override string ToString() =>
            "(" + string.Join(",", Values) + ")";

        #endregion
    }

    [Serializable]
    public sealed class MultiKey<T> : IEquatable<MultiKey<T>>
    {
        #region Properties

        public T[] Values => _values;

        #endregion

        #region Fields

        private readonly T[] _values;
        private int _index;
        private int? _hash;

        #endregion

        #region Construction

        public MultiKey(int length)
        {
            _index = 0;
            _values = new T[length];
        }

        public MultiKey(params T[] values)
        {
            if (values.IsEmpty())
                throw new ArgumentNullException(nameof(values));

            _index = int.MaxValue;
            _values = values;
        }

        public MultiKey(IEnumerable<T> values)
        {
            if (values == null || !values.Any())
                throw new ArgumentNullException(nameof(values));

            _index = int.MaxValue;
            _values = values.ToArray();
        }

        #endregion

        #region Methods

        public void Add(T value)
        {
            _values[_index++] = value;
        }

        public override int GetHashCode()
        {
            return _hash ?? (int)(_hash = Values.Aggregate(17, (current, t) => current * 23 + t.GetHashCode()));
        }

        public override bool Equals(object obj) =>
            Equals(obj as MultiKey<T>);

        public bool Equals(MultiKey<T> key)
        {
            if (key == null || key.Values.Length != Values.Length)
                return false;

            for (var i = 0; i < Values.Length; i++)
                if (!Values[i].Equals(key.Values[i]))
                    return false;

            return true;
        }

        public override string ToString() =>
            "(" + string.Join(",", Values) + ")";

        #endregion
    }

    [Serializable]
    public class MultiKey<T1, T2> : IEquatable<MultiKey<T1, T2>>
    {
        #region Properties

        public T1 Key1 { get; private set; }
        public T2 Key2 { get; private set; }

        #endregion

        #region Fields

        private int _hash;

        #endregion

        #region Construction

        public MultiKey(T1 key1, T2 key2)
        {
            Key1 = key1;
            Key2 = key2;

            _hash = 17;
            _hash = _hash * 23 + Key1.GetHashCode();
            _hash = _hash * 23 + Key2.GetHashCode();
        }

        #endregion

        #region Methods

        public override int GetHashCode() => _hash;

        public override bool Equals(object obj) =>
            Equals(obj as MultiKey<T1, T2>);

        public bool Equals(MultiKey<T1, T2> key) => key != null
            && EqualityComparer<T1>.Default.Equals(Key1, key.Key1)
            && EqualityComparer<T2>.Default.Equals(Key2, key.Key2);

        public override string ToString() =>
            $"({Key1},{Key2})";

        #endregion
    }

    [Serializable]
    public class MultiKey<T1, T2, T3> : IEquatable<MultiKey<T1, T2, T3>>
    {
        #region Properties

        public T1 Key1 { get; private set; }
        public T2 Key2 { get; private set; }
        public T3 Key3 { get; private set; }

        #endregion

        #region Fields

        private int _hash;

        #endregion

        #region Construction

        public MultiKey(T1 key1, T2 key2, T3 key3)
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;

            _hash = 17;
            _hash = _hash * 23 + Key1.GetHashCode();
            _hash = _hash * 23 + Key2.GetHashCode();
            _hash = _hash * 23 + Key3.GetHashCode();
        }

        #endregion

        #region Methods

        public override int GetHashCode() => _hash;

        public override bool Equals(object obj) =>
            Equals(obj as MultiKey<T1, T2, T3>);

        public bool Equals(MultiKey<T1, T2, T3> other) => other != null
            && EqualityComparer<T1>.Default.Equals(Key1, other.Key1)
            && EqualityComparer<T2>.Default.Equals(Key2, other.Key2)
            && EqualityComparer<T3>.Default.Equals(Key3, other.Key3);

        public override string ToString() =>
            $"({Key1},{Key2},{Key3})";

        #endregion
    }
}
