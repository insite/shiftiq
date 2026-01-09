using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Sdk.UI;

namespace InSite.Admin.Contacts.Memberships.Controls
{
    public class SummaryItem
    {
        public string Text { get; set; }
        public int Count { get; set; }
    }

    public class SummaryTable
    {
        public string Name { get; set; }
        public int Sum => Items != null ? Items.Sum(x => x.Count) : 0;
        public IEnumerable<SummaryItem> Items { get; set; }
    }

    public partial class SummaryTables : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TableRepeater.ItemDataBound += TableRepeater_ItemDataBound;
        }

        private void TableRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var tableData = (SummaryTable)e.Item.DataItem;
            var hasData = tableData.Items.Any();

            var itemsRepeater = (Repeater)e.Item.FindControl("TableRepeater");
            var noDataMessage = e.Item.FindControl("NoDataMessage");

            itemsRepeater.Visible = hasData;
            itemsRepeater.DataSource = tableData.Items;
            itemsRepeater.DataBind();

            noDataMessage.Visible = !hasData;
        }

        public bool LoadData(IEnumerable<MembershipSearchResult> memberships)
        {
            var tables = new List<SummaryTable>();

            { // Group Label
                var items = memberships
                    .GroupBy(x => x.GroupLabel ?? "(None)")
                    .Select(x => new SummaryItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                tables.Add(new SummaryTable
                {
                    Name = "Group Tag",
                    Items = items
                });
            }

            { // Group Name
                var items = memberships
                    .GroupBy(x => x.GroupName)
                    .Select(x => new SummaryItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                tables.Add(new SummaryTable
                {
                    Name = "Group Name",
                    Items = items
                });
            }

            { // Membership Function
                var items = memberships
                    .GroupBy(x => x.MembershipFunction ?? "(None)")
                    .Select(x => new SummaryItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                tables.Add(new SummaryTable
                {
                    Name = "Membership Function",
                    Items = items
                });
            }

            { // Year Effective
                var items = memberships
                    .GroupBy(x => x.MembershipAssigned.Year.ToString())
                    .Select(x => new SummaryItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .ToArray();

                tables.Add(new SummaryTable
                {
                    Name = "Effective Year",
                    Items = items.OrderBy(x => x.Text)
                });
            }

            TableRepeater.DataSource = tables;
            TableRepeater.DataBind();

            return tables.Any(x => x.Items.Any(y => y.Count > 0));
        }
    }
}