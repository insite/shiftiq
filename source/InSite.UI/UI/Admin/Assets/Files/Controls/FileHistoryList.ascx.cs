using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Files.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Assets.Files.Controls
{
    public partial class FileHistoryList : BaseUserControl
    {
        class ChangeItem
        {
            public string FieldName { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }

            public string OldValueHtml => GetHtml(OldValue);
            public string NewValueHtml => GetHtml(NewValue);

            static string GetHtml(string value) => !string.IsNullOrEmpty(value) ? $"<b>{value}</b>" : "<i>None</i>";
        }

        class HistoryItem
        {
            public string Date { get; set; }
            public string User { get; set; }
            public List<ChangeItem> Changes { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HistoryList.ItemDataBound += HistoryList_ItemDataBound;
        }

        private void HistoryList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var historyItem = (HistoryItem)e.Item.DataItem;

            var createdPanel = e.Item.FindControl("CreatedPanel");
            createdPanel.Visible = historyItem.Changes == null;

            var changeRepeater = (Repeater)e.Item.FindControl("ChangeRepeater");
            changeRepeater.Visible = historyItem.Changes != null;
            changeRepeater.DataSource = historyItem.Changes;
            changeRepeater.DataBind();
        }

        public void BindModelToControls(FileStorageModel model)
        {
            var historyItems = GetHistoryItems(model);

            HistoryList.Visible = historyItems.Count > 0;
            HistoryList.DataSource = historyItems;
            HistoryList.DataBind();

            EmptyMessage.Visible = historyItems.Count == 0;
        }

        private List<HistoryItem> GetHistoryItems(FileStorageModel model)
        {
            var activities = ServiceLocator.FileSearch.GetFileActivities(model.FileIdentifier);
            var users = new Dictionary<Guid, string>();

            var list = activities
                .Select(x => new HistoryItem
                {
                    Date = x.ActivityTime.Format(User.TimeZone, true),
                    User = GetUser(x.UserIdentifier),
                    Changes = GetChanges(x)
                })
                .ToList();

            list.Add(new HistoryItem
            {
                Date = model.Uploaded.Format(User.TimeZone, true),
                User = GetUser(model.UserIdentifier),
                Changes = null
            });

            return list;

            string GetUser(Guid userIdentifier)
            {
                if (users.TryGetValue(userIdentifier, out var name))
                    return name;

                name = UserSearch.Select(userIdentifier)?.FullName ?? "Unknown";
                users.Add(userIdentifier, name);
                return name;
            }
        }

        private static List<ChangeItem> GetChanges(FileActivity activity)
        {
            return activity.ActivityChanges
                .Select(c => new ChangeItem
                {
                    FieldName = c.FieldName,
                    OldValue = c.IsDate ? GetDate(c.OldDate) : c.OldValue,
                    NewValue = c.IsDate ? GetDate(c.NewDate) : c.NewValue
                })
                .ToList();
        }

        private static string GetDate(DateTimeOffset? value)
        {
            return value.HasValue ? value.Value.FormatDateOnly(User.TimeZone) : null;
        }
    }
}