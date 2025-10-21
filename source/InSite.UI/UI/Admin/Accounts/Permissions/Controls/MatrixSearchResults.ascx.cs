using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Sdk.UI;

using BoundField = System.Web.UI.WebControls.BoundField;

namespace InSite.Admin.Accounts.Permissions.Controls
{
    public partial class MatrixSearchResults : SearchResultsGridViewController<MatrixFilter>
    {
        #region Classes

        [Serializable]
        public class ActionItem
        {
            public Guid ActionIdentifier { get; set; }
            public string ActionName { get; set; }

            public string DataField => "Column" + ActionIdentifier.GetHashCode();
        }

        private class MatrixResult
        {
            public DataTable Table { get; set; }
            public List<QGroup> Groups { get; set; }
        }

        private class PermissionItem
        {
            public Guid GroupIdentifier { get; set; }
            public Guid ActionIdentifier { get; set; }
            public string Permissions { get; set; }
        }

        #endregion

        #region Properties

        private static readonly string[] _colors = new[]
        {
            "#ffffff", // None
            "#f3f7fe", // Read
            "#fff9f2", // Write
            "#fff9f2", // Create
            "#ecfbf7", // Delete
            "#fef1f4", // Administrate
            "#fef1f4", // Configure
        };

        public List<ActionItem> ActionItems
        {
            get => (List<ActionItem>)ViewState[nameof(ActionItems)];
            set => ViewState[nameof(ActionItems)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.EnablePaging = false;
            Grid.RowDataBound += Grid_RowDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnLoad(e);
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsContentItem(e))
                OnItemDataBound(e);
            else
                OnHeaderDataBound(e);
        }

        private void OnItemDataBound(GridViewRowEventArgs e)
        {
            var row = e.Row;
            var rowIndex = e.Row.RowIndex;
            var k = new List<string>();
            foreach (TableCell item in e.Row.Cells)
            {
                k.Add(item.Text);
            }
            e.Row.Cells[0].Text = $"<span title='{row.Cells[0].Text}'style=\"font-weight: normal;\">{row.Cells[0].Text}</span>";

            for (int i = 1; i < row.Cells.Count; i++)
            {
                var value = row.Cells[i].Text;
                var ddl = GetDropDownList(rowIndex, i, value);
                var cell = e.Row.Cells[i];

                var color = _colors[0];
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains("F"))
                        color = _colors[6];
                    else if (value.Contains("A"))
                        color = _colors[5];

                    else if (value.Contains("D"))
                        color = _colors[4];
                    else if (value.Contains("C"))
                        color = _colors[3];

                    else if (value.Contains("W"))
                        color = _colors[2];
                    else if (value.Contains("R"))
                        color = _colors[1];
                }

                cell.Text = ddl;
                cell.Style[HtmlTextWriterStyle.BackgroundColor] = color;
                cell.Style[HtmlTextWriterStyle.TextAlign] = "center";
            }
        }

        private void OnHeaderDataBound(GridViewRowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                var cell = e.Row.Cells[i];
                var value = cell.Text;
                cell.Text = $"<span title='{value}' style=\"width: 200px; display: flex; font-weight: normal;\">{value}</span>";
            }
        }

        #endregion

        #region Searching

        protected override int SelectCount(MatrixFilter filter)
        {
            var groupFilter = new QGroupFilter
            {
                GroupIdentifier = filter.GroupIdentifier,
                OrganizationIdentifier = filter.OrganizationIdentifier,
                GroupType = filter.GroupType
            };
            return ServiceLocator.GroupSearch.CountGroups(groupFilter);
        }

        protected override IListSource SelectData(MatrixFilter filter)
        {
            var result = CreateMatrix(filter);

            SetupGrid();

            var toolkitInitScript = "var _toolkits = [" + string.Join(",", ActionItems.Select(x => $"'{x.ActionIdentifier}'")) + "];";
            var groupInitScript = "var _groups = [" + string.Join(",", result.Groups.Select(x => $"'{x.GroupIdentifier}'")) + "];";
            var colorInitScript = "var _colors = [" + string.Join(",", _colors.Select(x => $"'{x}'")) + "];";

            DictionaryLiteral.Text = toolkitInitScript + "\r\n" + groupInitScript + "\r\n" + colorInitScript;

            return result.Table;
        }

        private void SetupGrid()
        {
            //Grid.MasterTableView.GroupByExpressions.Clear();
            //No Support

            //Grid.ClientSettings.Scrolling.FrozenColumnsCount = 1;
            //https://stackoverflow.com/questions/13529532/freeze-asp-net-grid-view-column

            Grid.Columns.Clear();


            var groupNameColumn = new BoundField
            {
                DataField = "GroupName",
                HeaderText = "GroupName"
            };
            groupNameColumn.ItemStyle.Font.Italic = false;

            Grid.Columns.Add(groupNameColumn);

            foreach (var toolkit in ActionItems)
            {
                var col = new BoundField
                {
                    HeaderText = toolkit.ActionName,
                    DataField = toolkit.DataField
                };
                Grid.Columns.Add(col);
            }
        }

        public override IListSource GetExportData(MatrixFilter filter, bool empty)
        {
            return CreateMatrix(filter).Table;
        }

        #endregion

        #region Save

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["isPageAjax"], out var isAjax) || !isAjax)
                return;

            var changesJson = Page.Request.Form["changes"];
            var changes = ParsePermissionChanges(changesJson);

            foreach (var change in changes)
                SavePermissionChange(change);

            Response.Clear();
            Response.Write("OK");
            Response.End();
        }

        private static void SavePermissionChange(PermissionItem item)
        {
            if (string.IsNullOrEmpty(item.Permissions))
            {
                TGroupPermissionStore.Delete(item.GroupIdentifier, item.ActionIdentifier);
                return;
            }

            var allowExecute = item.Permissions.Contains("X");

            var allowRead = item.Permissions.Contains("R");
            var allowWrite = item.Permissions.Contains("W");

            var allowCreate = item.Permissions.Contains("C");
            var allowDelete = item.Permissions.Contains("D");

            var allowAdministrate = item.Permissions.Contains("A");
            var allowConfigure = item.Permissions.Contains("F");

            TGroupPermissionStore.Update(item.GroupIdentifier, item.ActionIdentifier, "Action", allowExecute, allowRead, allowWrite, allowCreate, allowDelete, allowAdministrate, allowConfigure);
        }

        private static List<PermissionItem> ParsePermissionChanges(string changesJson)
        {
            var changesList = JsonConvert.DeserializeObject<List<string>>(changesJson);
            var result = new List<PermissionItem>();

            foreach (var change in changesList)
            {
                var parts = change.Split(new[] { ':' });
                var item = new PermissionItem
                {
                    GroupIdentifier = Guid.Parse(parts[0]),
                    ActionIdentifier = Guid.Parse(parts[1]),
                    Permissions = parts[2]
                };

                result.Add(item);
            }

            return result;
        }

        #endregion

        #region Load

        private MatrixResult CreateMatrix(MatrixFilter filter)
        {
            var table = new DataTable();

            LoadToolkits(filter, table);

            var groups = LoadGroups(filter, table);

            LoadPermissions(filter, table, groups);

            return new MatrixResult { Table = table, Groups = groups };
        }

        private void LoadToolkits(MatrixFilter filter, DataTable table)
        {
            ActionItems = TActionSearch.Search(x => x.ActionType == "Permission")
                .Where(x => filter.ActionIdentifier == null || x.ActionIdentifier == filter.ActionIdentifier)
                .Select(x => new ActionItem
                {
                    ActionIdentifier = x.ActionIdentifier,
                    ActionName = x.ActionName
                })
                .OrderBy(x => x.ActionName)
                .ToList();

            table.Columns.Add(new DataColumn("GroupName", typeof(string)));
            foreach (var toolkit in ActionItems)
                table.Columns.Add(new DataColumn(toolkit.DataField, typeof(string)));
        }

        private List<QGroup> LoadGroups(MatrixFilter filter, DataTable table)
        {
            var groupFilter = new QGroupFilter
            {
                GroupIdentifier = filter.GroupIdentifier,
                OrganizationIdentifier = filter.OrganizationIdentifier,
                GroupType = filter.GroupType
            };

            var groups = ServiceLocator.GroupSearch.GetGroups(groupFilter).OrderBy(x => x.GroupName).ToList();
            foreach (var group in groups)
            {
                var row = table.NewRow();
                row["GroupName"] = group.GroupName;

                table.Rows.Add(row);
            }

            return groups;
        }

        private void LoadPermissions(MatrixFilter filter, DataTable table, List<QGroup> groups)
        {
            var toolkitMap = new Dictionary<Guid, int>();
            for (int i = 0; i < ActionItems.Count; i++)
                toolkitMap.Add(ActionItems[i].ActionIdentifier, i);

            var groupMap = new Dictionary<Guid, int>();
            for (int i = 0; i < groups.Count; i++)
                groupMap.Add(groups[i].GroupIdentifier, i);

            var permissionFilter = new TGroupActionFilter
            {
                GroupIdentifier = filter.GroupIdentifier,
                OrganizationIdentifier = filter.OrganizationIdentifier,
                ActionIdentifier = filter.ActionIdentifier,
                GroupType = filter.GroupType
            };
            var permissions = TGroupPermissionSearch.Select(permissionFilter);
            foreach (var item in permissions.GetList())
            {
                var permission = (TGroupPermissionSearchResult)item;

                if (!groupMap.ContainsKey(permission.GroupIdentifier) || !toolkitMap.ContainsKey(permission.ObjectIdentifier))
                    continue;

                var rowIndex = groupMap[permission.GroupIdentifier];
                var colIndex = toolkitMap[permission.ObjectIdentifier];
                var row = table.Rows[rowIndex];
                var value = GetValueString(permission);

                row[colIndex + 1] = value;
            }
        }

        private static string GetValueString(TGroupPermissionSearchResult permission)
        {
            if (permission.AllowConfigure)
                return "RWCDAF";

            if (permission.AllowAdministrate)
                return "RWCDA";

            if (permission.AllowDelete)
                return "RWCD";

            if (permission.AllowCreate)
                return "RWC";

            if (permission.AllowWrite)
                return "RW";

            if (permission.AllowRead)
                return "R";

            return null;
        }

        private static string GetDropDownList(int rowIndex, int colIndex, string selectedValue)
        {
            var empty = string.IsNullOrEmpty(selectedValue) ? " selected" : "";

            var r = selectedValue == "R" ? " selected" : "";
            var rw = selectedValue == "RW" ? " selected" : "";

            var rwc = selectedValue == "RWC" ? " selected" : "";
            var rwcd = selectedValue == "RWCD" ? " selected" : "";

            var rwcda = selectedValue == "RWCDA" ? " selected" : "";
            var rwcdaf = selectedValue == "RWCDAF" ? " selected" : "";

            var result = new StringBuilder();
            result.AppendLine($"<select class=\"permission-selector\" data-row=\"{rowIndex}\" data-col=\"{colIndex}\">");
            result.AppendLine($"<option value=\"\"{empty}></option>");

            result.AppendLine($"<option value=\"R\"{r}>R</option>");
            result.AppendLine($"<option value=\"RW\"{rw}>RW</option>");

            result.AppendLine($"<option value=\"RWC\"{rwc}>RWC</option>");
            result.AppendLine($"<option value=\"RWCD\"{rwcd}>RWCD</option>");

            result.AppendLine($"<option value=\"RWCDA\"{rwcda}>RWCDA</option>");
            result.AppendLine($"<option value=\"RWCDAF\"{rwcdaf}>RWCDAF</option>");

            result.AppendLine("</select>");

            return result.ToString();
        }

        #endregion
    }
}