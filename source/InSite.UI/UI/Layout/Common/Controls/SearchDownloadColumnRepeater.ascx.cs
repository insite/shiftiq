using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class SearchDownloadColumnRepeater : BaseUserControl
    {
        #region Classes

        public sealed class GroupInfo
        {
            public string Name => _group.GroupName;

            public GroupColumn[] Columns { get; }

            private DownloadColumnGroup _group;

            public GroupInfo(DownloadColumnGroup group, GroupColumn[] columns)
            {
                _group = group;
                Columns = columns;
            }
        }

        public sealed class GroupColumn
        {
            public int Index => _index;

            public DownloadColumn Column => _column.Info;

            private int _index;
            private DownloadColumnState _column;

            public GroupColumn(int index, DownloadColumnState column)
            {
                _index = index;
                _column = column;
            }
        }

        #endregion

        #region Properties

        public int ItemsCount
        {
            get => (int)(ViewState[nameof(ItemsCount)] ?? 0);
            set => ViewState[nameof(ItemsCount)] = value;
        }

        public BaseSearchDownload.JsonColumnState[] State
        {
            get => JsonConvert.DeserializeObject<BaseSearchDownload.JsonColumnState[]>(StateInput.Value);
            set
            {
                if (value.Length != ItemsCount)
                    throw ApplicationError.Create("StateLength != ItemsCount");

                StateInput.Value = JsonConvert.SerializeObject(value);
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var group = (GroupInfo)e.Item.DataItem;

            var repeater = (Repeater)e.Item.FindControl("Repeater");
            repeater.DataSource = group.Columns.Where(x => x.Column.Visible);
            repeater.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(SearchDownloadColumnRepeater),
                "init_" + ClientID,
                $"inSite.common.searchDownload.init(" +
                $"{HttpUtility.JavaScriptStringEncode(StateInput.ClientID, true)}," +
                $"{HttpUtility.JavaScriptStringEncode(Repeater.ClientID, true)});",
                true);
        }

        public void LoadData(IEnumerable<DownloadColumnState> columns)
        {
            ItemsCount = 0;

            var groups = new List<GroupInfo>();
            {
                DownloadColumnGroup group = null;
                List<GroupColumn> list = null;

                foreach (var c in columns)
                {
                    if (group == null || group.GroupName != c.Group.GroupName)
                    {
                        if (group != null)
                            groups.Add(new GroupInfo(group, list.ToArray()));

                        group = c.Group;
                        list = new List<GroupColumn>();
                    }

                    list.Add(new GroupColumn(ItemsCount++, c));
                }

                if (list.Count > 0)
                    groups.Add(new GroupInfo(group, list.ToArray()));
            }

            Repeater.DataSource = groups.Where(x => x.Columns.Any(y => y.Column.Visible));
            Repeater.DataBind();
        }

        protected string GetTranslatedColumnTitle()
        {
            var title = DataBinder.Eval(Page.GetDataItem(), "Column.Title") as string;
            return Translate(title);
        }
    }
}