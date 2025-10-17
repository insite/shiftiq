using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Shift.Common
{
    [Serializable]
    public class PivotDimension : IReadOnlyList<string>
    {
        #region Properties

        /// <summary>
        /// Each dimension has a unique name.
        /// </summary>
        public string Name { get; private set; }

        public int Count => _unitsList.Count;

        public string this[int index] => _unitsList[index];

        #endregion

        #region Fields

        private List<string> _unitsList;
        private Dictionary<string, int> _unitsDictionary;

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new dimension with a specific name.
        /// </summary>
        public PivotDimension(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            _unitsList = new List<string>();
            _unitsDictionary = new Dictionary<string, int>();
        }

        #endregion

        #region Methods (public)

        /// <summary>
        /// Adds a value to a named dimension.
        /// </summary>
        public void AddUnit(string unit)
        {
            if (string.IsNullOrEmpty(unit))
                throw new ArgumentNullException(nameof(unit));

            // The values within a dimension must be unique.
            if (Contains(unit))
                throw new DuplicatePivotValueException(unit);

            _unitsList.Add(unit);
            _unitsDictionary.Add(unit, _unitsList.Count - 1);
        }

        /// <summary>
        /// Returns true if the dimension contains a specific value; returns false otherwise.
        /// </summary>
        public bool Contains(string unit) => _unitsDictionary.ContainsKey(unit);

        public int GetIndex(string unit) => _unitsDictionary[unit];

        /// <summary>
        /// The string representation for a dimension should look like this: Name { UnitA, UnitB, UnitC }
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Name);
            sb.Append(" { ");

            var lastIndex = _unitsList.Count - 1;
            for (var i = 0; i <= lastIndex; i++)
            {
                sb.Append(_unitsList[i]);
                if (i != lastIndex)
                    sb.Append(", ");
            }

            sb.Append(" }");

            return sb.ToString();
        }

        public IEnumerator<string> GetEnumerator() => _unitsList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
