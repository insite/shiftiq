using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public static class GridViewExtensions
    {
        public static T GetDataKey<T>(this GridView grid, GridViewRow row, string name) =>
            (T)grid.DataKeys[row.RowIndex][name];

        public static T GetDataKey<T>(this GridView grid, GridViewRow row) =>
            (T)grid.DataKeys[row.RowIndex][0];

        public static T GetDataKey<T>(this GridView grid, int rowIndex, string name) =>
            (T)grid.DataKeys[rowIndex][name];

        public static T GetDataKey<T>(this GridView grid, int rowIndex) =>
            (T)grid.DataKeys[rowIndex][0];

        public static T GetDataKey<T>(this GridView grid, GridViewDeleteEventArgs args, string name) =>
            (T)grid.DataKeys[args.RowIndex][name];

        public static T GetDataKey<T>(this GridView grid, GridViewDeleteEventArgs args) =>
            (T)grid.DataKeys[args.RowIndex][0];

        public static T GetDataKey<T>(this GridView grid, GridViewCommandEventArgs args, string name)
        {
            var row = GetRow(args);
            return row != null && row.RowType == DataControlRowType.DataRow
                ? (T)grid.DataKeys[row.RowIndex][name]
                : default;
        }

        public static T GetDataKey<T>(this GridView grid, GridViewCommandEventArgs args)
        {
            var row = GetRow(args);
            return row != null && row.RowType == DataControlRowType.DataRow
                ? (T)grid.DataKeys[row.RowIndex][0]
                : default;
        }

        public static GridViewRow GetRow(GridViewCommandEventArgs args) =>
            GetRow((Control)args.CommandSource);

        public static GridViewRow GetRow(Control ctrl)
        {
            var current = ctrl;

            while (current != null)
            {
                if (current is GridViewRow row)
                    return row;

                current = ctrl.NamingContainer;
            }

            return default;
        }

        public static IEnumerable<DataControlField> FindAllByName(this DataControlFieldCollection columns, string fieldName, bool ignoreCase = false) =>
            FindAllByName(columns, fieldName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        public static IEnumerable<DataControlField> FindAllByName(this DataControlFieldCollection columns, string fieldName, StringComparison comparison)
        {
            if (fieldName.IsNotEmpty())
            {
                foreach (DataControlField field in columns)
                {
                    if (field is IGridFieldHasName fieldWithName && fieldWithName.FieldName.IsNotEmpty() && fieldWithName.FieldName.Equals(fieldName, comparison))
                        yield return field;
                }
            }
        }

        public static DataControlField FindByName(this DataControlFieldCollection columns, string fieldName, bool ignoreCase = false) =>
            FindByName(columns, fieldName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        public static DataControlField FindByName(this DataControlFieldCollection columns, string fieldName, StringComparison comparison)
        {
            if (fieldName.IsNotEmpty())
            {
                for (var i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    if (column is IGridFieldHasName fieldWithName && fieldWithName.FieldName.IsNotEmpty() && fieldWithName.FieldName.Equals(fieldName, comparison))
                        return column;
                }
            }

            return null;
        }

        public static IEnumerable<DataControlField> FindAllByHeaderText(this DataControlFieldCollection columns, string headerText, bool ignoreCase = false) =>
            FindAllByHeaderText(columns, headerText, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        public static IEnumerable<DataControlField> FindAllByHeaderText(this DataControlFieldCollection columns, string headerText, StringComparison comparison)
        {
            if (headerText.IsNotEmpty())
            {
                for (var i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    if (column.HeaderText.IsNotEmpty() && column.HeaderText.Equals(headerText, comparison))
                        yield return column;
                }
            }
        }

        public static DataControlField FindByHeaderText(this DataControlFieldCollection columns, string headerText, bool ignoreCase = false) =>
            FindByHeaderText(columns, headerText, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        public static DataControlField FindByHeaderText(this DataControlFieldCollection columns, string headerText, StringComparison comparison)
        {
            if (headerText.IsNotEmpty())
            {
                for (var i = 0; i < columns.Count; i++)
                {
                    var column = columns[i];
                    if (column.HeaderText.IsNotEmpty() && column.HeaderText.Equals(headerText, comparison))
                        return column;
                }
            }

            return null;
        }
    }
}