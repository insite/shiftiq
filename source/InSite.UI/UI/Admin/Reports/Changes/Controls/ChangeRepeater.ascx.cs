using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Changes;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Reports.Changes.Controls
{
    public partial class ChangeRepeater : BaseUserControl
    {
        #region Classes

        public class DataItem
        {
            public Guid AggregateIdentifier { get; set; }
            public string AssetNumber { get; set; }
            public int Version { get; set; }
            public string User { get; set; }
            public string Time { get; set; }
            public string Name { get; set; }
            public string Data { get; set; }

            public static DataItem FromChange(IChange change, TimeZoneInfo timeZone)
            {
                var user = UserSearch.Bind(
                    change.OriginUser,
                    x => new { x.UserIdentifier, x.FullName });

                var item = new DataItem
                {
                    AggregateIdentifier = change.AggregateIdentifier,
                    Version = change.AggregateVersion,
                    User = user != null ? user.FullName : "(Unknown)",
                    Time = change.ChangeTime.Format(timeZone, true, false),
                    Name = change.GetType().Name
                };

                item.Data = change.Serialize(item.AggregateIdentifier, item.Version).ChangeData;
                return item;
            }

            public static DataItem[] FromChanges(IEnumerable<IChange> changes, TimeZoneInfo timeZone, bool html = true)
            {
                var users = UserSearch
                    .Bind(
                        x => new { x.UserIdentifier, x.FullName },
                        new UserFilter { IncludeUserIdentifiers = changes.Select(x => x.OriginUser).Distinct().ToArray() })
                    .ToDictionary(x => x.UserIdentifier, x => x);

                var result = new List<DataItem>();

                foreach (var e in changes.OrderByDescending(x => x.ChangeTime).ThenByDescending(x => x.AggregateVersion))
                {
                    var item = new DataItem
                    {
                        AggregateIdentifier = e.AggregateIdentifier,
                        Version = e.AggregateVersion,
                        User = users.TryGetValue(e.OriginUser, out var user) ? user.FullName : "(Unknown)",
                        Time = e.ChangeTime.Format(timeZone, html, false),
                        Name = e.GetType().Name,
                    };

                    item.Data = e.Serialize(item.AggregateIdentifier, item.Version).ChangeData;
                    result.Add(item);
                }

                return result.ToArray();
            }
        }

        #endregion

        #region Properties

        private Guid AggregateID
        {
            get => (Guid)ViewState[nameof(AggregateID)];
            set => ViewState[nameof(AggregateID)] = value;
        }

        public bool ShowAssetNumber { get; set; }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            StyleLiteral.ContentKey = GetType().FullName;
            ScriptLiteral.ContentKey = GetType().FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "init_" + ClientID,
                $"changeRepeater.init('{AggregateID}','{Repeater.ClientID}','{DataViewerWindow.ClientID}');",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region Methods (data binding)

        public void LoadData(Guid aggregateId)
        {
            var changes = ServiceLocator.ChangeStore.GetChanges(aggregateId, -1);

            LoadData(aggregateId, changes);
        }

        public void LoadData(Guid aggregateId, IEnumerable<IChange> changes)
        {
            AggregateID = aggregateId;

            var dataItems = DataItem.FromChanges(changes, User.TimeZone);

            Repeater.DataSource = dataItems.OrderBy(x => x.Version);
            Repeater.DataBind();
        }

        public void LoadData(Guid aggregateId, IEnumerable<DataItem> dataItems)
        {
            AggregateID = aggregateId;

            Repeater.DataSource = dataItems.OrderBy(x => x.Version);
            Repeater.DataBind();
        }

        #endregion
    }
}
