using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionLikertDetails : BaseUserControl
    {
        #region Classes

        [Serializable]
        private class RowInfo
        {
            public int InfoId { get; }
            public Guid? EntityId { get; }

            public int Sequence { get; set; }
            public Guid Standard { get; set; }
            public ContentTitle Content { get; set; }
            public bool IsLocked { get; }

            public RowInfo(int controlId)
            {
                InfoId = controlId;
                Content = new ContentTitle();
            }

            public RowInfo(int controlId, LikertRow entity, bool isLocked)
            {
                InfoId = controlId;
                EntityId = entity.Identifier;
                Standard = entity.Standard;
                Content = entity.Content.Clone();
                IsLocked = isLocked;
            }
        }

        [Serializable]
        private class ColumnInfo
        {
            public int InfoId { get; }
            public Guid? EntityId { get; }

            public int Sequence { get; set; }
            public string Letter => Calculator.ToBase26(Sequence);
            public ContentTitle Content { get; set; }
            public bool IsLocked { get; }

            public ColumnInfo(int controlId)
            {
                InfoId = controlId;
                Content = new ContentTitle();
            }

            public ColumnInfo(int controlId, LikertColumn entity, bool isLocked)
            {
                InfoId = controlId;
                EntityId = entity.Identifier;
                Content = entity.Content.Clone();
                IsLocked = isLocked;
            }
        }

        [Serializable]
        private class MatrixData
        {
            public IReadOnlyList<RowInfo> Rows => _rows.AsReadOnly();
            public IReadOnlyList<ColumnInfo> Columns => _columns.AsReadOnly();

            private readonly List<RowInfo> _rows;
            private readonly List<ColumnInfo> _columns;
            private readonly Dictionary<(int RowId, int ColumnId), decimal> _points;

            public MatrixData(Question question)
            {
                if (question == null)
                {
                    _rows = new List<RowInfo>();
                    _columns = new List<ColumnInfo>();
                    _points = new Dictionary<(int, int), decimal>();

                    AddRow();
                    AddRow();
                    AddRow();

                    AddColumn();
                    AddColumn();
                    AddColumn();
                    AddColumn();
                }
                else
                {
                    var likert = question.Likert;
                    var lockedOptionKeys = ServiceLocator.AttemptSearch.GetAttemptExistOptionKeys(question.Identifier).ToHashSet();
                    var lockedOptions = likert.Options.Where(x => lockedOptionKeys.Contains(x.Number)).ToArray();

                    var lockedRows = lockedOptions.Select(x => x.RowIdentifier).ToHashSet();
                    var lockedColumns = lockedOptions.Select(x => x.ColumnIdentifier).ToHashSet();

                    _rows = likert.Rows.Select(x => new RowInfo(GetNextControlId(), x, lockedRows.Contains(x.Identifier))).ToList();
                    _columns = likert.Columns.Select(x => new ColumnInfo(GetNextControlId(), x, lockedColumns.Contains(x.Identifier))).ToList();

                    UpdateRowSequence();
                    UpdateColumnSequence();

                    var rowMapping = _rows.ToDictionary(x => x.EntityId.Value, x => x.InfoId);
                    var columnMapping = _columns.ToDictionary(x => x.EntityId.Value, x => x.InfoId);

                    _points = likert.Options.ToDictionary(
                        x => (rowMapping[x.RowIdentifier], columnMapping[x.ColumnIdentifier]),
                        x => x.Points);
                }
            }

            #region Methods (rows)

            public void AddRow()
            {
                var row = new RowInfo(GetNextControlId());
                row.Sequence = _rows.Count + 1;

                _rows.Add(row);

                foreach (var column in _columns)
                    _points.Add((row.InfoId, column.InfoId), 0);
            }

            public RowInfo GetRow(int rowId)
            {
                return _rows.FirstOrDefault(x => x.InfoId == rowId);
            }

            public void DeleteRow(int rowId)
            {
                var removed = _rows.RemoveAll(x => x.InfoId == rowId);
                if (removed == 0)
                    return;

                if (removed != 1)
                    throw ApplicationError.Create("Duplicate row identifier found: {0}", rowId);

                var keys = _points.Keys.Where(x => x.RowId == rowId).ToArray();
                foreach (var key in keys)
                    _points.Remove(key);

                UpdateRowSequence();
            }

            public void ReorderRows(IEnumerable<int> order)
            {
                var items = order.Distinct().Select(x => GetRow(x)).Where(x => x != null).ToArray();

                if (items.Length != Rows.Count)
                    throw ApplicationError.Create("{0} does't match the matrix state", nameof(order));

                _rows.Clear();
                _rows.AddRange(items);

                UpdateRowSequence();
            }

            public void UpdateRowSequence()
            {
                for (var i = 0; i < _rows.Count; i++)
                    _rows[i].Sequence = i + 1;
            }

            #endregion

            #region Methods (columns)

            public void AddColumn()
            {
                var column = new ColumnInfo(GetNextControlId());
                column.Sequence = _columns.Count + 1;

                _columns.Add(column);

                foreach (var row in _rows)
                    _points.Add((row.InfoId, column.InfoId), 0);
            }

            public ColumnInfo GetColumn(int columnId)
            {
                return _columns.FirstOrDefault(x => x.InfoId == columnId);
            }

            public void DeleteColumn(int columnId)
            {
                var removed = _columns.RemoveAll(x => x.InfoId == columnId);
                if (removed == 0)
                    return;

                if (removed != 1)
                    throw ApplicationError.Create("Duplicate column identifier found: {0}", columnId);

                var keys = _points.Keys.Where(x => x.ColumnId == columnId).ToArray();
                foreach (var key in keys)
                    _points.Remove(key);

                UpdateColumnSequence();
            }

            public void ReorderColumns(IEnumerable<int> order)
            {
                var items = order.Distinct().Select(x => GetColumn(x)).Where(x => x != null).ToArray();

                if (items.Length != Columns.Count)
                    throw ApplicationError.Create("{0} does't match the matrix state", nameof(order));

                _columns.Clear();
                _columns.AddRange(items);

                UpdateColumnSequence();
            }

            public void UpdateColumnSequence()
            {
                for (var i = 0; i < _columns.Count; i++)
                    _columns[i].Sequence = i + 1;
            }

            #endregion

            #region Methods (options)

            public decimal GetPoints(int rowId, int columnId) => GetPoints((rowId, columnId));

            public decimal GetPoints((int RowId, int ColumnId) key)
            {
                if (!_points.TryGetValue(key, out var points))
                    throw ApplicationError.Create("Option not found: RowId={0}, ColumnId={1}", key.RowId, key.ColumnId);

                return points;
            }

            public IEnumerable<decimal> GetRowPoints(int rowId) => _columns.Select(x => GetPoints(rowId, x.InfoId));

            public IEnumerable<decimal> GetColumnPoints(int columnId) => _rows.Select(x => GetPoints(x.InfoId, columnId));

            public void SetPoints(int rowId, int columnId, decimal value) => SetPoints((rowId, columnId), value);

            public void SetPoints((int RowId, int ColumnId) key, decimal value)
            {
                if (!_points.ContainsKey(key))
                    throw ApplicationError.Create("Option not found: RowId={0}, ColumnId={1}", key.RowId, key.ColumnId);

                _points[key] = value;
            }

            #endregion

            #region Methods (other)

            private int _controlId = 0;
            private int GetNextControlId() => _controlId++;

            public LikertMatrix BuildMatrix()
            {
                var result = new LikertMatrix();
                var rowMapping = new List<(int InfoId, Guid EntityId)>();

                foreach (var info in _rows)
                {
                    var entity = info.EntityId.HasValue
                        ? result.AddRow(info.EntityId.Value)
                        : result.AddRow();
                    entity.Standard = info.Standard;
                    entity.Content = info.Content.Clone();

                    rowMapping.Add((info.InfoId, entity.Identifier));
                }

                foreach (var info in _columns)
                {
                    var entity = info.EntityId.HasValue
                        ? result.AddColumn(info.EntityId.Value)
                        : result.AddColumn();
                    entity.Content = info.Content.Clone();

                    foreach (var row in rowMapping)
                        entity.GetOption(row.EntityId).Points = GetPoints(row.InfoId, info.InfoId);
                }

                return result;
            }

            public IEnumerable<Command> GetCommands(Question question)
            {
                var matrix = BuildMatrix();
                var commands = new List<Command>();

                if (question.Likert.IsEmpty)
                    AddCommandsFromEmptyLikert(question, matrix, commands);
                else
                    AddCommandsFromNonEmptyLikert(question, matrix, commands);

                var bankId = question.Set.Bank.Identifier;
                commands.Add(new ChangeQuestionLikertOptions(bankId, question.Identifier, matrix.Options.ToArray()));

                return commands;
            }

            private void AddCommandsFromEmptyLikert(Question question, LikertMatrix matrix, List<Command> commands)
            {
                var bankId = question.Set.Bank.Identifier;

                foreach (var row in matrix.Rows)
                    commands.Add(new AddQuestionLikertRow(bankId, question.Identifier, row));

                foreach (var column in matrix.Columns)
                    commands.Add(new AddQuestionLikertColumn(bankId, question.Identifier, column));
            }

            private void AddCommandsFromNonEmptyLikert(Question question, LikertMatrix matrix, List<Command> commands)
            {
                var bankId = question.Set.Bank.Identifier;

                var otherRows = question.Likert.Rows.Select(x => x.Identifier).ToHashSet();
                var otherColumns = question.Likert.Columns.Select(x => x.Identifier).ToHashSet();

                foreach (var row in matrix.Rows)
                {
                    if (otherRows.Remove(row.Identifier))
                        commands.Add(new ChangeQuestionLikertRow(bankId, question.Identifier, row));
                    else
                        commands.Add(new AddQuestionLikertRow(bankId, question.Identifier, row));
                }

                foreach (var column in matrix.Columns)
                {
                    if (otherColumns.Remove(column.Identifier))
                        commands.Add(new ChangeQuestionLikertColumn(bankId, question.Identifier, column));
                    else
                        commands.Add(new AddQuestionLikertColumn(bankId, question.Identifier, column));
                }

                foreach (var id in otherRows)
                    commands.Add(new DeleteQuestionLikertRow(bankId, question.Identifier, id));

                foreach (var id in otherColumns)
                    commands.Add(new DeleteQuestionLikertColumn(bankId, question.Identifier, id));

                AddReorderCommand(question, matrix, commands);
            }

            private void AddReorderCommand(Question question, LikertMatrix matrix, List<Command> commands)
            {
                var bankId = question.Set.Bank.Identifier;
                var rows = matrix.Rows.Select((x, i) => (Id: x.Identifier, Index: i)).ToDictionary(x => x.Id, x => x.Index);
                var columns = matrix.Columns.Select((x, i) => (Id: x.Identifier, Index: i)).ToDictionary(x => x.Id, x => x.Index);

                commands.Add(new ReorderQuestionLikert(bankId, question.Identifier, rows, columns));
            }

            #endregion
        }

        #endregion

        #region Properties

        private MatrixData Data
        {
            get => (MatrixData)ViewState[nameof(Data)];
            set => ViewState[nameof(Data)] = value;
        }

        private List<int> RowKeys
        {
            get => (List<int>)ViewState[nameof(RowKeys)];
            set => ViewState[nameof(RowKeys)] = value;
        }

        private List<int> ColumnKeys
        {
            get => (List<int>)ViewState[nameof(ColumnKeys)];
            set => ViewState[nameof(ColumnKeys)] = value;
        }

        private List<List<(int RowId, int ColumId)>> OptionKeys
        {
            get => (List<List<(int RowId, int ColumId)>>)ViewState[nameof(OptionKeys)];
            set => ViewState[nameof(OptionKeys)] = value;
        }

        private Guid? FrameworkIdentifier
        {
            get { return (Guid?)ViewState[nameof(FrameworkIdentifier)]; }
            set { ViewState[nameof(FrameworkIdentifier)] = value; }
        }

        public bool IsReadOnly
        {
            get => (bool)(ViewState[nameof(IsReadOnly)] ?? false);
            set => ViewState[nameof(IsReadOnly)] = value;
        }

        #endregion

        #region Fields

        private bool _bindRows = false;
        private bool _bindColumns = false;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RowRepeater.DataBinding += RowRepeater_DataBinding;
            RowRepeater.ItemCreated += RowRepeater_ItemCreated;
            RowRepeater.ItemDataBound += RowRepeater_ItemDataBound;
            RowRepeater.ItemCommand += RowRepeater_ItemCommand;

            ColumnRepeater.DataBinding += ColumnRepeater_DataBinding;
            ColumnRepeater.ItemCreated += ColumnRepeater_ItemCreated;
            ColumnRepeater.ItemDataBound += ColumnRepeater_ItemDataBound;
            ColumnRepeater.ItemCommand += ColumnRepeater_ItemCommand;

            OptionsRowRepeater.DataBinding += OptionsRowRepeater_DataBinding;
            OptionsRowRepeater.ItemDataBound += OptionsRowRepeater_ItemDataBound;

            AddRowButton.Click += AddRowButton_Click;

            AddColumnButton.Click += AddColumnButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                ReadData();
        }

        protected override void SetupValidationGroup(string groupName)
        {
            if (!IsBaseControlLoaded)
                return;

            for (var i = 0; i < RowRepeater.Items.Count; i++)
                SetupRowRepeaterValidationGroup(RowRepeater.Items[i], groupName);

            for (var i = 0; i < ColumnRepeater.Items.Count; i++)
                SetupColumnRepeaterValidationGroup(ColumnRepeater.Items[i], groupName);
        }

        private void SetupRowRepeaterValidationGroup(RepeaterItem item, string groupName)
        {
            var textValidator = (BaseValidator)item.FindControl("TextValidator");
            textValidator.ValidationGroup = groupName;
        }

        private void SetupColumnRepeaterValidationGroup(RepeaterItem item, string groupName)
        {
            var textValidator = (BaseValidator)item.FindControl("TextValidator");
            textValidator.ValidationGroup = groupName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            AddRowButton.Visible = !IsReadOnly;
            AddColumnButton.Visible = !IsReadOnly;

            if (_bindRows)
                BindRows();

            if (_bindColumns)
                BindColumns();

            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(QuestionLikertDetails),
                "init_reorder",
                $"questionLikertDetails.initReorder('{RowRepeater.ClientID}','{RowsOrder.ClientID}');" +
                $"questionLikertDetails.initReorder('{ColumnRepeater.ClientID}','{ColumnsOrder.ClientID}');",
                true);
        }

        #endregion

        #region Event handlers

        private void RowRepeater_DataBinding(object sender, EventArgs e)
        {
            RowKeys = new List<int>();
            _bindRows = false;
        }

        private void RowRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            SetupRowRepeaterValidationGroup(e.Item, ValidationGroup);

            var standardField = e.Item.FindControl("StandardField");
            var standardIdentifier = (FindStandard)e.Item.FindControl("StandardIdentifier");

            standardField.Visible = FrameworkIdentifier.HasValue
                && Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection;
            standardIdentifier.Filter.RootStandardIdentifier = FrameworkIdentifier;
        }

        private void RowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (RowInfo)e.Item.DataItem;
            RowKeys.Add(dataItem.InfoId);
        }

        private void RowRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            Data.DeleteRow(RowKeys[e.Item.ItemIndex]);

            BindRows();
            BindOptions();
        }

        private void ColumnRepeater_DataBinding(object sender, EventArgs e)
        {
            ColumnKeys = new List<int>();
            _bindColumns = false;
        }

        private void ColumnRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (IsContentItem(e))
                SetupColumnRepeaterValidationGroup(e.Item, ValidationGroup);
        }

        private void ColumnRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (ColumnInfo)e.Item.DataItem;
            ColumnKeys.Add(dataItem.InfoId);
        }

        private void ColumnRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            Data.DeleteColumn(ColumnKeys[e.Item.ItemIndex]);

            BindColumns();
            BindOptions();
        }

        private void OptionsRowRepeater_DataBinding(object sender, EventArgs e)
        {
            OptionKeys = new List<List<(int, int)>>();
        }

        private void OptionsRowRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var row = (RowInfo)e.Item.DataItem;

            var rowKeys = new List<(int, int)>(Data.Columns.Count);
            OptionKeys.Add(rowKeys);

            var optionRepeater = (Repeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.DataSource = Data.Columns.Select(column =>
            {
                var key = (row.InfoId, column.InfoId);

                rowKeys.Add(key);

                return Data.GetPoints(key);
            });
            optionRepeater.DataBind();
        }

        private void AddColumnButton_Click(object sender, EventArgs e)
        {
            Data.AddColumn();

            BindColumns();
            BindOptions();
        }

        private void AddRowButton_Click(object sender, EventArgs e)
        {
            Data.AddRow();

            BindRows();
            BindOptions();
        }

        #endregion

        #region Setting and getting input values

        public void SetCompetencyFramework(Guid frameworkId)
        {
            FrameworkIdentifier = frameworkId == Guid.Empty ? (Guid?)null : frameworkId;
        }

        public void LoadData(Question question = null)
        {
            Data = new MatrixData(question);

            BindRows();
            BindColumns();
            BindOptions();
        }

        public LikertMatrix GetMatrix() => Data.BuildMatrix();

        public IEnumerable<Command> GetCommands(Question question) => Data == null ? null : Data.GetCommands(question);

        #endregion

        #region Methods (data binding)

        private void BindRows()
        {
            RowRepeater.DataSource = Data.Rows;
            RowRepeater.DataBind();
        }

        private void BindColumns()
        {
            ColumnRepeater.DataSource = Data.Columns;
            ColumnRepeater.DataBind();
        }

        private void BindOptions()
        {
            var hasOptions = Data.Columns.Count > 0 && Data.Rows.Count > 0;

            OptionsCard.Visible = hasOptions;

            if (hasOptions)
            {
                OptionsColumnRepeater.DataSource = Data.Columns;
                OptionsRowRepeater.DataSource = Data.Rows;
            }
            else
            {
                OptionsColumnRepeater.DataSource = null;
                OptionsRowRepeater.DataSource = null;
            }

            OptionsColumnRepeater.DataBind();
            OptionsRowRepeater.DataBind();
        }

        #endregion

        #region Read data

        private void ReadData()
        {
            ReadRows();
            ReadColumns();
            ReadOptions();

            ReorderRows();
            ReorderColumns();
        }

        private void ReadRows()
        {
            for (var i = 0; i < RowRepeater.Items.Count; i++)
            {
                var item = RowRepeater.Items[i];
                var row = Data.GetRow(RowKeys[i]);

                var text = (EditorTranslation)item.FindControl("Text");
                var standard = (FindStandard)item.FindControl("StandardIdentifier");

                row.Content.Title.Set(text.Text);
                row.Standard = standard.Value ?? Guid.Empty;
            }
        }

        private void ReadColumns()
        {
            for (var i = 0; i < ColumnRepeater.Items.Count; i++)
            {
                var item = ColumnRepeater.Items[i];
                var column = Data.GetColumn(ColumnKeys[i]);

                var text = (EditorTranslation)item.FindControl("Text");

                column.Content.Title.Set(text.Text);
            }
        }

        private void ReadOptions()
        {
            for (var x = 0; x < OptionsRowRepeater.Items.Count; x++)
            {
                var rowItem = OptionsRowRepeater.Items[x];

                var optionRepeater = (Repeater)rowItem.FindControl("OptionRepeater");
                for (var y = 0; y < optionRepeater.Items.Count; y++)
                {
                    var optionItem = optionRepeater.Items[y];
                    var key = OptionKeys[x][y];
                    var points = (NumericBox)optionItem.FindControl("Points");

                    Data.SetPoints(key, points.ValueAsDecimal ?? 0);
                }
            }
        }

        private void ReorderRows()
        {
            if (RowsOrder.Value.IsEmpty())
                return;

            var array = RowsOrder.Value.Split(';');
            var rows = new List<int>();

            for (var i = 0; i < array.Length; i++)
            {
                var rowId = int.Parse(array[i]);

                rows.Add(rowId);
            }

            Data.ReorderRows(rows);

            RowsOrder.Value = null;

            _bindRows = true;
        }

        private void ReorderColumns()
        {
            if (ColumnsOrder.Value.IsEmpty())
                return;

            var array = ColumnsOrder.Value.Split(';');
            var columns = new List<int>();

            for (var i = 0; i < array.Length; i++)
            {
                var columnId = int.Parse(array[i]);

                columns.Add(columnId);
            }

            Data.ReorderColumns(columns);

            ColumnsOrder.Value = null;

            _bindColumns = true;
        }

        #endregion

        #region Helper methods

        public string GetError()
        {
            if (Data.Rows.Count == 0)
                return "The question has no rows.";

            if (Data.Columns.Count == 0)
                return "The question has no columns.";

            if (Data.Rows.Any(x => Data.GetRowPoints(x.InfoId).All(y => y == 0)))
                return "Each row should contain at least one correct option.";

            return null;
        }

        #endregion
    }
}