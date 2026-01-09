using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shift.Common
{
    [Serializable]
    [JsonConverter(typeof(JsonPivotTableConverter))]
    public class PivotTable
    {
        #region Classes

        private class JsonPivotTableConverter : JsonConverter
        {
            public override bool CanConvert(Type type) => type == typeof(PivotTable);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return null;

                var jObj = JObject.Load(reader);
                var rows = jObj["rows"].ToObject<PivotDimensionList>();
                var columns = jObj["cols"].ToObject<PivotDimensionList>();
                var values = new Dictionary<MultiKey<MultiKey<string>>, int?>();

                var jValues = (JObject)jObj["values"];
                foreach (var jProp in jValues.Properties())
                {
                    var keys = jProp.Name.Split(':');
                    if (keys.Length != 2)
                        throw new ApplicationError("Invalid key value: " + jProp.Name);

                    var rowKey = GetKey(keys[0], rows);
                    var colKey = GetKey(keys[1], columns);
                    var cellKey = new MultiKey<MultiKey<string>>(rowKey, colKey);
                    var cellValue = (int)jProp.Value;

                    values.Add(cellKey, cellValue);
                }

                return new PivotTable(rows, columns, values);

                MultiKey<string> GetKey(string input, PivotDimensionList dimensions)
                {
                    var path = input.Split('.');
                    if (path.Length != dimensions.Count)
                        throw new ApplicationError("Invalid key value: " + input);

                    var key = new MultiKey<string>(path.Length);
                    for (var i = 0; i < path.Length; i++)
                    {
                        var unitIndex = int.Parse(path[i]);
                        key.Add(dimensions[i][unitIndex]);
                    }

                    return key;
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var table = (PivotTable)value;
                var pathBuilder = new StringBuilder();

                var values = new JObject();
                foreach (var kv in table._cellValues)
                {
                    if (kv.Value.HasValue)
                        values.Add(GetValueKey(kv.Key.Values[0].Values, kv.Key.Values[1].Values), kv.Value);
                }

                var result = new JObject
                {
                    { "rows", JObject.FromObject(table.RowDimensions, serializer) },
                    { "cols", JObject.FromObject(table.ColumnDimensions, serializer) },
                    { "values", values }
                };

                result.WriteTo(writer);

                string GetValueKey(string[] rowValues, string[] colValues)
                {
                    pathBuilder.Clear();

                    AddPath(table.RowDimensions, rowValues);
                    pathBuilder.Append(":");
                    AddPath(table.ColumnDimensions, colValues);

                    return pathBuilder.ToString();
                }

                void AddPath(PivotDimensionList dimensions, string[] keyValues)
                {
                    if (keyValues.Length == 0)
                        return;

                    for (var i = 0; i < keyValues.Length; i++)
                        pathBuilder.Append(dimensions[i].GetIndex(keyValues[i])).Append(".");

                    pathBuilder.Remove(pathBuilder.Length - 1, 1);
                }
            }
        }

        [Serializable]
        private class DimensionNode : IPivotDimensionNode
        {
            #region Properties

            public PivotTable Table { get; }

            public DimensionNode Parent { get; }

            public string Dimension => _dimensions[_dimensionIndex].Name;

            public bool IsRoot => _dimensionIndex == -1;

            public bool IsIndex => !IsRoot && _dimensionIndex >= _dimensions.Count - 1;

            public string Unit => _dimensions[_dimensionIndex][_unitIndex];

            public DimensionNode this[string unit]
            {
                get
                {
                    if (IsIndex)
                        throw new ArgumentOutOfRangeException(nameof(unit));

                    var dimension = _dimensions[_dimensionIndex + 1];
                    if (!dimension.Contains(unit))
                        throw new ArgumentOutOfRangeException(nameof(unit));

                    return new DimensionNode(Table, _dimensions, this, _getCells, _getCell, _dimensionIndex + 1, dimension.GetIndex(unit));
                }
            }

            public DimensionNode this[int index]
            {
                get
                {
                    if (IsIndex)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    var dimension = _dimensions[_dimensionIndex + 1];
                    if (index < 0 || index >= dimension.Count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    return new DimensionNode(Table, _dimensions, this, _getCells, _getCell, _dimensionIndex + 1, index);
                }
            }

            #endregion

            #region Fields

            private int _dimensionIndex = -1;
            private int _unitIndex = -1;
            private PivotDimensionList _dimensions;
            private Func<MultiKey<string>, IPivotCell[]> _getCells;
            private Func<MultiKey<string>, MultiKey<string>, IPivotCell> _getCell;

            #endregion

            #region Construction

            public DimensionNode(
                PivotTable table,
                PivotDimensionList dimensions,
                Func<MultiKey<string>, IPivotCell[]> getCells,
                Func<MultiKey<string>, MultiKey<string>, IPivotCell> getCell)
            {
                Table = table;
                _dimensions = dimensions;
                _getCell = getCell;
                _getCells = getCells;
            }

            public DimensionNode(
                PivotTable table,
                PivotDimensionList dimensions,
                DimensionNode parent,
                Func<MultiKey<string>, IPivotCell[]> getCells,
                Func<MultiKey<string>, MultiKey<string>, IPivotCell> getCell,
                int dimensionIndex,
                int unitIndex) : this(table, dimensions, getCells, getCell)
            {
                Parent = parent;
                _dimensionIndex = dimensionIndex;
                _unitIndex = unitIndex;
            }

            #endregion

            #region Methods

            public MultiKey<string> GetKey()
            {
                if (!IsIndex)
                    throw new InvalidOperationException();

                var key = new MultiKey<string>(_dimensions.Count);

                GetKey(key);

                return key;
            }

            public IEnumerator<DimensionNode> GetEnumerator()
            {
                if (_dimensions.Count == 0)
                    yield break;

                if (IsIndex)
                    throw new InvalidOperationException();

                var dimension = _dimensions[_dimensionIndex + 1];
                for (var i = 0; i < dimension.Count; i++)
                    yield return new DimensionNode(Table, _dimensions, this, _getCells, _getCell, _dimensionIndex + 1, i);
            }

            public DimensionNode[] GetIndexes()
            {
                var list = new List<DimensionNode>();

                GetIndexes(list);

                return list.ToArray();
            }

            public IPivotCell GetCell(IPivotDimensionNode index)
            {
                if (Table != index.Table)
                    throw new InvalidOperationException();

                return _getCell(GetKey(), index.GetKey());
            }

            public IPivotCell GetCell(MultiKey<string> key)
            {
                return _getCell(GetKey(), key);
            }

            public IPivotCell[] GetCells() => _getCells(GetKey());

            #endregion

            #region Methods (internal)

            private void GetKey(MultiKey<string> key)
            {
                if (_dimensionIndex > 0)
                    Parent.GetKey(key);

                key.Add(Unit);
            }

            private void GetIndexes(List<DimensionNode> list)
            {
                foreach (var level in this)
                {
                    if (level.IsIndex)
                        list.Add(level);
                    else
                        level.GetIndexes(list);
                }
            }

            #endregion

            #region IPivotLevelNode

            IPivotDimensionNode IPivotDimensionNode.Parent => Parent;

            IPivotDimensionNode IPivotDimensionNode.this[string unit] => this[unit];

            IPivotDimensionNode IPivotDimensionNode.this[int index] => this[index];

            IEnumerator<IPivotDimensionNode> IEnumerable<IPivotDimensionNode>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IPivotDimensionNode[] IPivotDimensionNode.GetIndexes() => GetIndexes();

            #endregion
        }

        [Serializable]
        private class PivotCell : IPivotCell
        {
            #region Properties

            public MultiKey<string> RowKey { get; }

            public MultiKey<string> ColumnKey { get; }

            public PivotTable Table { get; }

            public int? Value
            {
                get => Table.GetCellValue(RowKey, ColumnKey);
                set => Table.SetCellValue(RowKey, ColumnKey, value);
            }

            #endregion

            #region Construction

            public PivotCell(PivotTable table, MultiKey<string> rowKey, MultiKey<string> columnKey)
            {
                Table = table;
                RowKey = rowKey;
                ColumnKey = columnKey;
            }

            #endregion

            #region Methods

            public IPivotDimensionNode GetRow() => Table.GetRow(RowKey);

            public IPivotDimensionNode GetColumn() => Table.GetColumn(ColumnKey);

            #endregion
        }

        public class RenderOptions
        {
            #region Properties

            public bool RenderTotalColumn { get; set; }

            public bool RenderTotalRow { get; set; }

            public Func<PivotDimension, string> GetColumnAxisName
            {
                get => _getColumnAxisName ?? GetDefaultAxisName;
                set => _getColumnAxisName = value;
            }

            public Func<IPivotDimensionNode, string> GetColumnHeaderName
            {
                get => _getColumnHeaderName ?? GetDefaultHeaderName;
                set => _getColumnHeaderName = value;
            }

            public Func<PivotDimension, string> GetRowAxisName
            {
                get => _getRowAxisName ?? GetDefaultAxisName;
                set => _getRowAxisName = value;
            }

            public Func<IPivotDimensionNode, string> GetRowHeaderName
            {
                get => _getRowHeaderName ?? GetDefaultHeaderName;
                set => _getRowHeaderName = value;
            }

            public Func<IPivotCell, string> GetCellValue
            {
                get => _getCellValue ?? GetDefaultCellValue;
                set => _getCellValue = value;
            }

            public Func<IPivotCell[], string> GetColumnTotalValue
            {
                get => _getColumnTotalValue ?? GetDefaultColumnTotalValue;
                set => _getColumnTotalValue = value;
            }

            public Func<IPivotCell[], string> GetRowTotalValue
            {
                get => _getRowTotalValue ?? GetDefaultRowTotalValue;
                set => _getRowTotalValue = value;
            }

            public Func<IPivotCell[], string> GetGrandTotalValue
            {
                get => _getGrandTotalValue ?? GetDefaultGrandTotalValue;
                set => _getGrandTotalValue = value;
            }

            #endregion

            #region Fields

            private Func<PivotDimension, string> _getColumnAxisName = null;
            private Func<IPivotDimensionNode, string> _getColumnHeaderName = null;
            private Func<PivotDimension, string> _getRowAxisName = null;
            private Func<IPivotDimensionNode, string> _getRowHeaderName = null;
            private Func<IPivotCell, string> _getCellValue = null;
            private Func<IPivotCell[], string> _getColumnTotalValue = null;
            private Func<IPivotCell[], string> _getRowTotalValue = null;
            private Func<IPivotCell[], string> _getGrandTotalValue = null;

            #endregion

            #region Methods (default)

            private string GetDefaultAxisName(PivotDimension dimension) => dimension.Name;

            private string GetDefaultHeaderName(IPivotDimensionNode node) => node.Unit;

            private string GetDefaultCellValue(IPivotCell cell) => cell.Value.HasValue ? cell.Value.Value.ToString("n0") : string.Empty;

            private string GetDefaultColumnTotalValue(IPivotCell[] cells) => cells.Length == 0 ? string.Empty : cells.Sum(x => x.Value ?? 0).ToString("n0");

            private string GetDefaultRowTotalValue(IPivotCell[] cells) => cells.Length == 0 ? string.Empty : cells.Sum(x => x.Value ?? 0).ToString("n0");

            private string GetDefaultGrandTotalValue(IPivotCell[] cells) => cells.Length == 0 ? string.Empty : cells.Sum(x => x.Value ?? 0).ToString("n0");

            #endregion
        }

        #endregion

        #region Properties

        public IPivotDimensionNode Rows { get; }

        public IPivotDimensionNode Columns { get; }

        public PivotDimensionList RowDimensions { get; }

        public PivotDimensionList ColumnDimensions { get; }

        public bool IsEmpty => RowDimensions.Count == 0 || ColumnDimensions.Count == 0;

        #endregion

        #region Fields

        private Dictionary<MultiKey<MultiKey<string>>, int?> _cellValues;

        public int CellValueSum
        {
            get
            {
                return _cellValues.Sum(x => x.Value ?? 0);
            }
        }

        #endregion

        #region Construction

        private PivotTable(PivotDimensionList rowDimensions, PivotDimensionList columnDimensions, Dictionary<MultiKey<MultiKey<string>>, int?> values)
        {
            RowDimensions = rowDimensions;
            ColumnDimensions = columnDimensions;

            _cellValues = values;

            Rows = new DimensionNode(
                this,
                RowDimensions,
                (rowKey) => Columns.GetIndexes().Select(columnIndex => GetCell(rowKey, columnIndex.GetKey())).ToArray(),
                (row, column) => GetCell(row, column)
            );
            Columns = new DimensionNode(
                this,
                ColumnDimensions,
                (columnKey) => Rows.GetIndexes().Select(rowIndex => GetCell(rowIndex.GetKey(), columnKey)).ToArray(),
                (column, row) => GetCell(row, column)
            );
        }

        public PivotTable()
            : this(new PivotDimensionList(), new PivotDimensionList(), new Dictionary<MultiKey<MultiKey<string>>, int?>())
        {

        }

        #endregion

        #region Methods (public)

        /// <summary>
        /// Adds a row dimension to the pivot table.
        /// </summary>
        public void AddRow(string name, string[] units)
        {
            var dimension = RowDimensions.Add(name);
            foreach (var unit in units)
                dimension.AddUnit(unit);
        }

        public void AddRow(string name)
        {
            RowDimensions.Add(name);
        }

        /// <summary>
        /// Adds a column dimension to the pivot table.
        /// </summary>
        public void AddColumn(string name, string[] units)
        {
            var dimension = ColumnDimensions.Add(name);
            foreach (var unit in units)
                dimension.AddUnit(unit);
        }

        public void AddColumn(string name)
        {
            ColumnDimensions.Add(name);
        }

        public IPivotCell GetCell(MultiKey<string> row, MultiKey<string> column)
        {
            if (!RowDimensions.IsValidKey(row) || !ColumnDimensions.IsValidKey(column))
                throw new InvalidPivotKeyException();

            return new PivotCell(this, row, column);
        }

        /// <summary>
        /// Gets the value for the cell indexed by a specific row and column.
        /// </summary>
        public int? GetCellValue(MultiKey<string> row, MultiKey<string> column)
        {
            if (!RowDimensions.IsValidKey(row) || !ColumnDimensions.IsValidKey(column))
                throw new InvalidPivotKeyException();

            var compositeKey = GetCompositeKey(row, column);

            return _cellValues.ContainsKey(compositeKey) ? _cellValues[compositeKey] : null;
        }

        /// <summary>
        /// Sets the value for the cell indexed by a specific row and column.
        /// </summary>
        public void SetCellValue(MultiKey<string> row, MultiKey<string> column, int? value)
        {
            if (!RowDimensions.IsValidKey(row) || !ColumnDimensions.IsValidKey(column))
                throw new InvalidPivotKeyException();

            var compositeKey = GetCompositeKey(row, column);

            _cellValues[compositeKey] = value;
        }

        public IPivotDimensionNode GetRow(MultiKey<string> key)
        {
            var node = Rows;

            foreach (var unit in key.Values)
                node = node[unit];

            return node;
        }

        public IPivotDimensionNode GetColumn(MultiKey<string> key)
        {
            var node = Columns;

            foreach (var unit in key.Values)
                node = node[unit];

            return node;
        }

        #endregion

        #region Methods (HTML rendering)

        /// <summary>
        /// Returns the HTML for the pivot table.
        /// </summary>
        public string ToHtml(RenderOptions options = null)
        {
            if (options == null)
                options = new RenderOptions();

            var html = new StringBuilder();

            html.Append("<table class='table-pvt'><thead>");

            var columns = RenderHtmlTableHead(html, options);

            html.Append("</thead><tbody>");

            RenderHtmlTableBody(html, options, columns);

            html.Append("</tbody></table>");

            return html.ToString();
        }

        private IPivotDimensionNode[] RenderHtmlTableHead(StringBuilder html, RenderOptions options)
        {
            IPivotDimensionNode[] result = null;

            var columnLevels = new Stack<Tuple<IPivotDimensionNode, int>[]>();
            {
                var currentLevel = Columns.GetIndexes()
                    .Where(x => x.GetCells().Any(y => y.Value.HasValue))
                    .Select(x => new Tuple<IPivotDimensionNode, int>(x, 1))
                    .ToArray();

                result = currentLevel.Select(x => x.Item1).ToArray();
                columnLevels.Push(currentLevel);

                while (true)
                {
                    var upperLevel = currentLevel
                        .Where(x => !x.Item1.Parent.IsRoot)
                        .GroupBy(x => x.Item1.Parent)
                        .Select(x => new Tuple<IPivotDimensionNode, int>(x.Key, x.Sum(y => y.Item2)))
                        .ToArray();

                    if (upperLevel.Length == 0)
                        break;

                    columnLevels.Push(upperLevel);
                    currentLevel = upperLevel;
                }
            }

            html.Append("<tr>");

            html.AppendFormat("<th colspan='{0}' rowspan='{1}' class='pvt-spandrel'></th>", RowDimensions.Count, ColumnDimensions.Count);

            for (var i = 0; i < ColumnDimensions.Count; i++)
            {
                var isFirstColumn = i == 0;

                if (!isFirstColumn)
                    html.Append("</tr><tr>");

                html.Append("<th class='pvt-col-axis'>")
                    .Append(options.GetColumnAxisName(ColumnDimensions[i]))
                    .Append("</th>");

                var columns = columnLevels.Pop();
                var rowSpan = i == ColumnDimensions.Count - 1 ? 2 : 1;

                foreach (var column in columns)
                {
                    html.Append("<th class='pvt-col'");

                    if (rowSpan > 1)
                        html.AppendFormat(" rowspan='{0}'", rowSpan);

                    if (column.Item2 > 1)
                        html.AppendFormat(" colspan='{0}'", column.Item2);

                    html.AppendFormat(">{0}</th>", options.GetColumnHeaderName(column.Item1));
                }

                if (isFirstColumn && options.RenderTotalColumn)
                    html.AppendFormat("<th class='pvt-col-total' rowspan='{0}'>Total</th>", ColumnDimensions.Count + 1);
            }

            html.Append("</tr>");

            html.Append("<tr>");

            for (var i = 0; i < RowDimensions.Count; i++)
            {
                var dimension = RowDimensions[i];
                html.Append("<th class='pvt-row-axis'>")
                    .Append(options.GetRowAxisName(dimension))
                    .Append("</th>");
            }

            html.Append("<th class='pvt-spandrel'></th></tr>");

            return result;
        }

        private void RenderHtmlTableBody(StringBuilder html, RenderOptions options, IPivotDimensionNode[] columns)
        {
            html.Append("<tr>");

            var cells = new List<IPivotCell[]>();

            RenderHtmlTableRow(html, options, columns, Rows, cells);

            if (options.RenderTotalRow)
            {
                html.AppendFormat("<th class='pvt-row-total' colspan='{0}'>Total</th>", RowDimensions.Count + 1);

                var cellCount = cells.Count > 0 ? cells.Max(x => x.Length) : 0;
                for (var i = 0; i < cellCount; i++)
                {
                    var colCells = cells.Select(x => x[i]).ToArray();
                    html.Append("<td class='pvt-row-total-value'>")
                        .Append(options.GetRowTotalValue(colCells))
                        .Append("</td>");
                }

                if (options.RenderTotalColumn)
                    html.Append("<td class='pvt-grand-total-value'>")
                        .Append(options.GetGrandTotalValue(cells.SelectMany(x => x).ToArray()))
                        .Append("</td>");
            }
            else
            {
                html.Remove(html.Length - 9, 9);
            }

            html.Append("</tr>");
        }

        private void RenderHtmlTableRow(StringBuilder html, RenderOptions options, IPivotDimensionNode[] columns, IPivotDimensionNode rows, List<IPivotCell[]> cells)
        {
            foreach (var row in rows)
            {
                var rowSpan = 1;

                if (!row.IsIndex)
                {
                    var indexesCount = row.GetIndexes().Where(x => x.GetCells().Any(y => y.Value.HasValue)).Count();
                    if (indexesCount == 0)
                        continue;

                    rowSpan = indexesCount;
                }

                html.Append("<th class='pvt-row'");

                if (row.IsIndex)
                    html.Append(" colspan='2'");

                if (rowSpan > 1)
                    html.AppendFormat(" rowspan='{0}'", rowSpan);

                html.AppendFormat(">{0}</th>", options.GetRowHeaderName(row));

                if (row.IsIndex)
                {
                    var rowCells = new List<IPivotCell>();

                    foreach (var column in columns)
                    {
                        var cell = row.GetCell(column);

                        html.Append("<td class='pvt-value'>")
                            .Append(options.GetCellValue(cell))
                            .Append("</td>");

                        rowCells.Add(cell);
                    }

                    var rowCellsArray = rowCells.ToArray();
                    cells.Add(rowCellsArray);

                    if (options.RenderTotalColumn)
                        html.Append("<td class='pvt-col-total-value'>")
                            .Append(options.GetColumnTotalValue(rowCellsArray))
                            .Append("</td>");

                    html.Append("</tr><tr>");
                }
                else
                {
                    RenderHtmlTableRow(html, options, columns, row, cells);
                }
            }
        }

        #endregion

        #region Methods (helpers)

        private static MultiKey<MultiKey<string>> GetCompositeKey(MultiKey<string> row, MultiKey<string> column) =>
            new MultiKey<MultiKey<string>>(row, column);

        #endregion
    }
}
