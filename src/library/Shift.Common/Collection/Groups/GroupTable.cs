using System;
using System.Collections.Generic;

namespace Shift.Common
{
    public interface IGroupTable
    {

    }

    [Serializable]
    public class GroupTable<TColumn, TRow, TCell> : IGroupTable where TColumn : GroupLeaf, IComparable<TColumn> where TRow : GroupLeaf, IComparable<TRow>
    {
        #region Properties

        public bool HasData => _cells.Count > 0;

        public GroupInfoCollection<TColumn> Columns { get; }

        public GroupInfoCollection<TRow> Rows { get; }

        #endregion

        #region Fields

        private Dictionary<MultiKey, TCell> _cells = new Dictionary<MultiKey, TCell>();

        #endregion

        #region Construction

        public GroupTable()
        {
            Columns = new GroupInfoCollection<TColumn>(this, null);
            Rows = new GroupInfoCollection<TRow>(this, null);
        }

        #endregion

        #region Methods

        public TCell AddCell(GroupLeaf col, GroupLeaf row, Func<TCell> create, Action<TCell> collision = null)
        {
            TCell result = default(TCell);

            var key = new MultiKey(col.GetCellKey(), row.GetCellKey());
            if (!_cells.ContainsKey(key))
                _cells.Add(key, result = create());
            else
                collision?.Invoke(result = _cells[key]);

            return result;
        }

        public TCell GetCell(GroupLeaf col, GroupLeaf row)
        {
            var key = new MultiKey(col.GetCellKey(), row.GetCellKey());

            return _cells.ContainsKey(key) ? _cells[key] : default(TCell);
        }

        #endregion
    }
}
