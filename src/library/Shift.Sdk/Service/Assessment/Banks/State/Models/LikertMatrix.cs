using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class LikertMatrix
    {
        #region Classes

        [Serializable]
        private class OptionKey : MultiKey<Guid, Guid>
        {
            public OptionKey(Guid row, Guid column) : base(row, column)
            {

            }
        }

        [Serializable]
        private class InnerRow : LikertRow
        {
            [JsonIgnore]
            public override LikertMatrix Matrix => _matrix;

            [NonSerialized]
            private LikertMatrix _matrix;

            public InnerRow(Guid id) : base(id)
            {

            }

            public void SetContainer(LikertMatrix matrix)
            {
                if (_matrix != null)
                    throw ApplicationError.Create("Matrix is already assigned to this row");

                _matrix = matrix;
            }

            public void RemoveContainer() => _matrix = null;
        }

        [Serializable]
        private class InnerColumn : LikertColumn
        {
            [JsonIgnore]
            public override LikertMatrix Matrix => _matrix;

            [NonSerialized]
            private LikertMatrix _matrix;

            public InnerColumn(Guid id) : base(id)
            {

            }

            public void SetContainer(LikertMatrix matrix)
            {
                if (_matrix != null)
                    throw ApplicationError.Create("Matrix is already assigned to this column");

                _matrix = matrix;
            }

            public void RemoveContainer() => _matrix = null;
        }

        [Serializable]
        private class InnerOption : LikertOption
        {
            [JsonIgnore]
            public override LikertRow Row => _row;

            [JsonIgnore]
            public override LikertColumn Column => _column;

            [NonSerialized]
            private InnerRow _row;

            [NonSerialized]
            private InnerColumn _column;

            public InnerOption(Guid row, Guid column) : base(row, column)
            {

            }

            public void SetContainer(InnerRow row, InnerColumn column)
            {
                if (row.Identifier != RowIdentifier)
                    throw ApplicationError.Create("RowIdentifier does not match");

                if (column.Identifier != ColumnIdentifier)
                    throw ApplicationError.Create("ColumnIdentifier does not match");

                _row = row;
                _column = column;
            }

            public void RemoveContainer()
            {
                _row = null;
                _column = null;
            }
        }

        #endregion

        #region Properties

        public Question Question { get; set; }

        public IEnumerable<LikertRow> Rows => _rows.AsEnumerable();

        public IEnumerable<LikertColumn> Columns => _columns.AsEnumerable();

        public IEnumerable<LikertOption> Options => _options.AsEnumerable();

        public int RowCount => _rows.Count;

        public int ColumnCount => _columns.Count;

        public int OptionCount => _options.Count;

        public bool IsEmpty => _rows.Count == 0 && _columns.Count == 0;

        public bool HasOptions => _rows.Count > 0 && _columns.Count > 0;

        public decimal? Points => HasOptions ? _rows.Sum(x => x.Points.Value) : (decimal?)null;

        #endregion

        #region Fields

        [JsonProperty(PropertyName = nameof(Rows))]
        private List<InnerRow> _rows = new List<InnerRow>();

        [JsonProperty(PropertyName = nameof(Columns))]
        private List<InnerColumn> _columns = new List<InnerColumn>();

        [JsonProperty(PropertyName = nameof(Options))]
        private List<InnerOption> _options = new List<InnerOption>();

        [NonSerialized]
        private Dictionary<OptionKey, InnerOption> _optionsIndex = new Dictionary<OptionKey, InnerOption>();

        #endregion

        #region Methods (row)

        public LikertRow AddRow() => AddRow(UuidFactory.Create());

        public LikertRow AddRow(Guid id)
        {
            if (ContainsId(id))
                throw ApplicationError.Create("Identifier duplicate: " + id);

            var row = new InnerRow(id);
            row.SetContainer(this);

            _rows.Add(row);

            foreach (var column in _columns)
                AddOption(row, column);

            return row;
        }

        public void RemoveRow(Guid id)
        {
            var row = (InnerRow)GetRow(id)
                ?? throw ApplicationError.Create("Row not found: " + id);

            foreach (var column in _columns)
                RemoveOption(row, column);

            _rows.Remove(row);

            row.RemoveContainer();
        }

        public void ReorderRows(IDictionary<Guid, int> order)
        {
            _rows = _rows.OrderBy(x => order.GetOrDefault(x.Identifier, x.Index)).ToList();
        }

        public LikertRow GetRow(int index) => _rows[index];

        public LikertRow GetRow(Guid id) => _rows.FirstOrDefault(x => x.Identifier == id);

        public int GetIndex(LikertRow row) => _rows.IndexOf((InnerRow)row);

        #endregion

        #region Methods (columns)

        public LikertColumn AddColumn() => AddColumn(UuidFactory.Create());

        public LikertColumn AddColumn(Guid id)
        {
            if (ContainsId(id))
                throw ApplicationError.Create("Identifier duplicate: " + id);

            var column = new InnerColumn(id);
            column.SetContainer(this);

            _columns.Add(column);

            foreach (var row in _rows)
                AddOption(row, column);

            return column;
        }

        public void RemoveColumn(Guid id)
        {
            var column = (InnerColumn)GetColumn(id)
                ?? throw ApplicationError.Create("Column not found: " + id);

            foreach (var row in _rows)
                RemoveOption(row, column);

            _columns.Remove(column);

            column.RemoveContainer();
        }

        public void ReorderColumns(IDictionary<Guid, int> order)
        {
            _columns = _columns.OrderBy(x => order.GetOrDefault(x.Identifier, x.Index)).ToList();
        }

        public LikertColumn GetColumn(int index) => _columns[index];

        public LikertColumn GetColumn(Guid id) => _columns.FirstOrDefault(x => x.Identifier == id);

        public int GetIndex(LikertColumn column) => _columns.IndexOf((InnerColumn)column);

        #endregion

        #region Methods (options)

        private InnerOption AddOption(InnerRow row, InnerColumn column)
        {
            var option = new InnerOption(row.Identifier, column.Identifier);
            option.SetContainer(row, column);

            var key = new OptionKey(row.Identifier, column.Identifier);

            _options.Add(option);
            _optionsIndex.Add(key, option);

            return option;
        }

        private void RemoveOption(InnerRow row, InnerColumn column)
        {
            var key = new OptionKey(row.Identifier, column.Identifier);
            var option = _optionsIndex[key];

            _options.Remove(option);
            _optionsIndex.Remove(key);

            option.RemoveContainer();
        }

        public LikertOption GetOption(LikertOption option) => GetOption(option.RowIdentifier, option.ColumnIdentifier);

        public LikertOption GetOption(LikertRow row, LikertColumn column) => GetOption(row.Identifier, column.Identifier);

        public LikertOption GetOption(Guid row, Guid column)
        {
            var key = new OptionKey(row, column);

            return _optionsIndex[key];
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (_optionsIndex == null)
                _optionsIndex = new Dictionary<OptionKey, InnerOption>();

            RestoreReferences();
        }

        public bool ShouldSerialize_rows()
        {
            return _rows.Count > 0;
        }

        public bool ShouldSerialize_columns()
        {
            return _columns.Count > 0;
        }

        public bool ShouldSerialize_options()
        {
            return _options.Count > 0;
        }

        #endregion

        #region Methods (other)

        private bool ContainsId(Guid id) => _rows.Any(x => x.Identifier == id) || _columns.Any(x => x.Identifier == id);

        private void RestoreReferences()
        {
            foreach (var option in _options)
            {
                var key = new OptionKey(option.RowIdentifier, option.ColumnIdentifier);
                _optionsIndex.Add(key, option);
            }

            foreach (var column in _columns)
                column.SetContainer(this);

            foreach (var row in _rows)
            {
                row.SetContainer(this);

                foreach (var column in _columns)
                {
                    var option = (InnerOption)GetOption(row.Identifier, column.Identifier);
                    option.SetContainer(row, column);
                }
            }
        }

        public LikertMatrix Clone(bool cloneIdentifiers = true)
        {
            var clone = new LikertMatrix();

            foreach (var thisColumn in _columns)
            {
                var cloneColumn = cloneIdentifiers
                    ? clone.AddColumn(thisColumn.Identifier)
                    : clone.AddColumn();

                thisColumn.CopyTo(cloneColumn);
            }

            foreach (var thisRow in _rows)
            {
                var cloneRow = cloneIdentifiers
                    ? clone.AddRow(thisRow.Identifier)
                    : clone.AddRow();

                thisRow.CopyTo(cloneRow);

                for (var i = 0; i < clone._columns.Count; i++)
                {
                    var thisColumn = _columns[i];
                    var cloneColumn = clone._columns[i];
                    var thisOption = GetOption(thisRow, thisColumn);
                    var cloneOption = clone.GetOption(cloneRow, cloneColumn);

                    thisOption.CopyTo(cloneOption);
                }
            }

            return clone;
        }

        public bool IsEqual(LikertMatrix other, bool compareIdentifiers = true)
        {
            if (this._rows.Count != other._rows.Count || this._columns.Count != other._columns.Count)
                return false;

            for (var i = 0; i < _columns.Count; i++)
            {
                var thisColumn = this._columns[i];
                var otherColumn = other._columns[i];

                if (!thisColumn.IsEqual(otherColumn, compareIdentifiers))
                    return false;
            }

            for (var i = 0; i < _rows.Count; i++)
            {
                var thisRow = this._rows[i];
                var otherRow = other._rows[i];

                if (!thisRow.IsEqual(otherRow, compareIdentifiers))
                    return false;

                if (!compareIdentifiers)
                {
                    for (var j = 0; j < _columns.Count; j++)
                    {
                        var thisColumn = this._columns[j];
                        var otherColumn = other._columns[j];

                        var thisOption = this.GetOption(thisRow, thisColumn);
                        var otherOption = other.GetOption(otherRow, otherColumn);

                        if (!thisOption.IsEqual(otherOption, compareIdentifiers))
                            return false;
                    }
                }
            }

            if (compareIdentifiers)
            {
                foreach (var thisOption in this._options)
                {
                    var otherOption = other.GetOption(thisOption);

                    if (!thisOption.IsEqual(otherOption, compareIdentifiers))
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
