using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Quizzes.Read;
using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Admin.Assessments.Quizzes.Controls
{
    public partial class EditorTypingAccuracy : BaseUserControl
    {
        #region Constants

        private const int MaxColumnCount = 3;
        private const int MaxRowCount = 2;

        #endregion

        #region Properties

        private List<TQuizTypingAccuracyQuestion> DataQuestions
        {
            get => (List<TQuizTypingAccuracyQuestion>)ViewState[nameof(DataQuestions)];
            set => ViewState[nameof(DataQuestions)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionRepeater.DataBinding += QuestionRepeater_DataBinding;
            QuestionRepeater.ItemDataBound += QuestionRepeater_ItemDataBound;
            QuestionRepeater.ItemCreated += QuestionRepeater_ItemCreated;
            QuestionRepeater.ItemCommand += QuestionRepeater_ItemCommand;

            ValuesValidator.ServerValidate += ValuesValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                ReadItems();
        }

        protected override void SetupValidationGroup(string groupName)
        {
            ValuesValidator.ValidationGroup = groupName;

            if (!IsBaseControlLoaded)
                return;

            foreach (RepeaterItem questionItem in QuestionRepeater.Items)
            {
                var columnRepeater = GetColumnRepeater(questionItem);
                foreach (RepeaterItem columnItem in columnRepeater.Items)
                {
                    var rowRepeater = GetRowRepeater(columnItem);
                    foreach (RepeaterItem rowItem in rowRepeater.Items)
                        SetupRowValidator(rowItem, groupName);
                }
            }
        }

        private void SetupRowValidator(RepeaterItem item, string groupName)
        {
            var labelValidator = (BaseValidator)item.FindControl("LabelRequiredValidator");
            labelValidator.ValidationGroup = groupName;

            var valuesValidator = (BaseValidator)item.FindControl("ValuesRequiredValidator");
            valuesValidator.ValidationGroup = groupName;
        }

        private void ReadItems()
        {
            if (DataQuestions == null)
                return;

            for (var questionIndex = 0; questionIndex < DataQuestions.Count; questionIndex++)
            {
                var columns = DataQuestions[questionIndex].Columns;
                var questionItem = QuestionRepeater.Items[questionIndex];
                var columnRepeater = GetColumnRepeater(questionItem);

                for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var column = columns[columnIndex];
                    var columnItem = columnRepeater.Items[columnIndex];
                    var rowRepeater = GetRowRepeater(columnItem);
                    var rows = column.Rows;

                    for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var row = rows[rowIndex];
                        var rowItem = rowRepeater.Items[rowIndex];
                        var label = (ITextBox)rowItem.FindControl("Label");
                        var values = (ITextBox)rowItem.FindControl("Values");

                        row.Label = label.Text;
                        row.Values = values.Text.Split(new[] { '\r', '\n' }).Select(x => x.Trim()).Where(x => x.IsNotEmpty()).ToList();
                    }
                }
            }
        }

        #endregion

        #region Event handlers

        private void QuestionRepeater_DataBinding(object sender, EventArgs e)
        {
            var n = 0;

            QuestionRepeater.DataSource = DataQuestions.Select(x =>
            {
                n++;
                return new
                {
                    QuestionNumber = n,
                    CanDelete = DataQuestions.Count > 1,
                    CanAdd = n == DataQuestions.Count,
                    Columns = x.Columns
                };
            }).ToList();
        }

        private void QuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var columns = (IEnumerable<TQuizTypingAccuracyColumn>)DataBinder.Eval(e.Item.DataItem, "Columns");

            BindColumnRepeater(e.Item, columns);
        }

        private void QuestionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var columnRepeater = GetColumnRepeater(e.Item);
            columnRepeater.ItemDataBound += ColumnRepeater_ItemDataBound;
            columnRepeater.ItemCreated += ColumnRepeater_ItemCreated;
            columnRepeater.ItemCommand += ColumnRepeater_ItemCommand;
        }

        private void QuestionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.Item.ItemIndex < 0)
                return;

            if (e.CommandName == "AddColumn")
            {
                var question = DataQuestions[e.Item.ItemIndex];
                if (question.Columns.Count >= MaxColumnCount)
                    return;

                var newColumn = new TQuizTypingAccuracyColumn();
                EnsureHasItems(newColumn);

                question.Columns.Add(newColumn);

                BindColumnRepeater(e.Item, question.Columns);
            }
            else if (e.CommandName == "DeleteQuestion")
            {
                DataQuestions.RemoveAt(e.Item.ItemIndex);
                QuestionRepeater.DataBind();
            }
            else if (e.CommandName == "AddQuestion")
            {
                var newQuestion = new TQuizTypingAccuracyQuestion();
                EnsureHasItems(newQuestion);
                DataQuestions.Add(newQuestion);
                QuestionRepeater.DataBind();
            }
        }

        private void ColumnRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var rowRepeater = GetRowRepeater(e.Item);
            rowRepeater.ItemCreated += RowRepeater_ItemCreated;
            rowRepeater.ItemCommand += RowRepeater_ItemCommand;
        }

        private void RowRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            SetupRowValidator(e.Item, ValidationGroup);
        }

        private void RowRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                var columnItem = (RepeaterItem)e.Item.NamingContainer.NamingContainer;
                var questionItem = (RepeaterItem)columnItem.NamingContainer.NamingContainer;
                var column = DataQuestions[questionItem.ItemIndex].Columns[columnItem.ItemIndex];

                column.Rows.RemoveAt(e.Item.ItemIndex);

                EnsureHasItems(column);
                BindRowRepeater(columnItem, column.Rows);
            }
        }

        private void ColumnRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var rows = (IEnumerable<TQuizTypingAccuracyRow>)DataBinder.Eval(e.Item.DataItem, "Rows");

            BindRowRepeater(e.Item, rows);
        }

        private void ColumnRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddRow")
            {
                var questionItem = (RepeaterItem)e.Item.NamingContainer.NamingContainer;

                var column = DataQuestions[questionItem.ItemIndex].Columns[e.Item.ItemIndex];
                if (column.Rows.Count >= MaxRowCount)
                    return;

                var newRow = new TQuizTypingAccuracyRow();

                EnsureHasItems(newRow);

                column.Rows.Add(newRow);

                BindRowRepeater(e.Item, column.Rows);
            }
            else if (e.CommandName == "DeleteColumn")
            {
                var questionItem = (RepeaterItem)e.Item.NamingContainer.NamingContainer;
                var columns = DataQuestions[questionItem.ItemIndex].Columns;

                columns.RemoveAt(e.Item.ItemIndex);

                EnsureHasItems();

                BindColumnRepeater(questionItem, columns);
            }
        }

        private void ValuesValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            for (var questionIndex = 0; questionIndex < DataQuestions.Count; questionIndex++)
            {
                var columns = DataQuestions[questionIndex].Columns;

                for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var column = columns[columnIndex];
                    var rows = column.Rows;

                    for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var row = rows[rowIndex];
                        if (row.Values.IsEmpty())
                        {
                            var validator = (BaseValidator)source;
                            validator.ErrorMessage = $"Required field: Field Values ({Calculator.ToBase26(columnIndex)}.{rowIndex + 1})";
                            args.IsValid = false;
                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region Set/Get Data

        public void SetData(IEnumerable<TQuizTypingAccuracyQuestion> items)
        {
            DataQuestions = new List<TQuizTypingAccuracyQuestion>(items);
            EnsureHasItems();
            QuestionRepeater.DataBind();
        }

        public IEnumerable<TQuizTypingAccuracyQuestion> GetData()
        {
            return DataQuestions.Select(x => x.Clone()).ToArray();
        }

        #endregion

        #region Helpers

        private Repeater GetRowRepeater(RepeaterItem columnItem)
        {
            return (Repeater)columnItem.FindControl("RowRepeater");
        }

        private InSite.Common.Web.UI.Button GetAddRowButton(RepeaterItem questionItem)
        {
            return (InSite.Common.Web.UI.Button)questionItem.FindControl("AddRowButton");
        }

        private Repeater GetColumnRepeater(RepeaterItem questionItem)
        {
            return (Repeater)questionItem.FindControl("ColumnRepeater");
        }

        private InSite.Common.Web.UI.Button GetAddColumnButton(RepeaterItem questionItem)
        {
            return (InSite.Common.Web.UI.Button)questionItem.FindControl("AddColumnButton");
        }

        private void BindColumnRepeater(RepeaterItem questionItem, IEnumerable<TQuizTypingAccuracyColumn> dataSource)
        {
            var columnRepeater = GetColumnRepeater(questionItem);

            columnRepeater.DataSource = dataSource.Select((x, i) => new
            {
                Letter = Calculator.ToBase26(i + 1),
                Rows = x.Rows
            });
            columnRepeater.DataBind();

            var addButton = GetAddColumnButton(questionItem);
            addButton.Visible = columnRepeater.Items.Count < MaxColumnCount;
        }

        private void BindRowRepeater(RepeaterItem columnItem, IEnumerable<TQuizTypingAccuracyRow> dataSource)
        {
            var rowRepeater = GetRowRepeater(columnItem);
            rowRepeater.DataSource = dataSource.Select((x, i) => new
            {
                ColumnLetter = Calculator.ToBase26(columnItem.ItemIndex + 1),
                RowNumber = i + 1,
                Label = x.Label,
                Values = string.Join(Environment.NewLine, x.Values)
            });
            rowRepeater.DataBind();

            var addButton = GetAddRowButton(columnItem);
            addButton.Visible = rowRepeater.Items.Count < MaxRowCount;
        }

        private void EnsureHasItems()
        {
            if (DataQuestions.Count == 0)
                DataQuestions.Add(new TQuizTypingAccuracyQuestion());

            foreach (var column in DataQuestions)
                EnsureHasItems(column);
        }

        private static void EnsureHasItems(TQuizTypingAccuracyQuestion question)
        {
            if (question.Columns.Count == 0)
                question.Columns.Add(new TQuizTypingAccuracyColumn());

            foreach (var column in question.Columns)
                EnsureHasItems(column);
        }

        private static void EnsureHasItems(TQuizTypingAccuracyColumn column)
        {
            if (column.Rows.Count == 0)
                column.Rows.Add(new TQuizTypingAccuracyRow());

            foreach (var row in column.Rows)
                EnsureHasItems(row);
        }

        private static void EnsureHasItems(TQuizTypingAccuracyRow row)
        {
            if (row.Values.Count == 0)
                row.Values.Add(string.Empty);
        }

        #endregion
    }
}