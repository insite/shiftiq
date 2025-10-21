using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.NCSHA;

using Shift.Common;

namespace InSite.Custom.NCSHA.History.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<HistoryFilter>
    {
        #region Classes

        [Serializable]
        private class InnerFilter : HistoryFilter
        {
            public string EventGroup { get; set; }
        }

        #endregion

        #region Properties

        public override HistoryFilter Filter
        {
            get
            {
                var filter = new InnerFilter
                {
                    EventGroup = EventGroupSelector.Value,
                    EventTypeExclude = Forms.Search.DeclaredEvents.Where(x => !x.Visible).Select(x => x.Type.FullName).ToArray(),
                    UserId = UserSelector.ValueAsGuid,
                    UserEmail = UserEmail.Text,
                    RecordTime = new DateTimeOffsetRange
                    {
                        Since = RecordTimeSince.Value,
                        Before = RecordTimeBefore.Value,
                    }
                };

                if (EventNameSelector.Values.Any())
                    filter.EventTypeInclude = EventNameSelector.ValuesArray;
                else if (!string.IsNullOrEmpty(EventGroupSelector.Value))
                    filter.EventTypeInclude = EventNameSelector.FlattenOptions().Select(x => x.Value).ToArray();

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                var filter = (InnerFilter)value;
                if (filter == null)
                {
                    Clear();
                    return;
                }

                EventGroupSelector.Value = filter.EventGroup;
                OnEventGroupChanged();
                EventNameSelector.Values = filter.EventTypeInclude;
                UserSelector.ValueAsGuid = filter.UserId;
                UserEmail.Text = filter.UserEmail;

                RecordTimeSince.Value = filter.RecordTime?.Since;
                RecordTimeBefore.Value = filter.RecordTime?.Before;
            }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventGroupSelector.AutoPostBack = true;
            EventGroupSelector.ValueChanged += EventGroupSelector_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                EventGroupSelector.LoadItems(
                    Forms.Search.DeclaredEvents.Where(x => x.Visible).Select(x => x.Group).Distinct().OrderBy(x => x));

                OnEventGroupChanged();

                var userIds = HistoryRepository.Distinct(x => x.UserId, x => true);
                if (userIds.Length > 0)
                {
                    var users = UserSearch.Bind(
                        x => new { x.UserIdentifier, x.FullName },
                        new UserFilter { IncludeUserIdentifiers = userIds.ToArray() });
                    if (users.Length > 0)
                        UserSelector.LoadItems(users.OrderBy(x => x.FullName), "UserIdentifier", "FullName");
                }
            }
        }

        #endregion

        #region Event handlers

        private void EventGroupSelector_ValueChanged(object sender, EventArgs e) =>
            OnEventGroupChanged();

        private void OnEventGroupChanged()
        {
            EventNameSelector.Items.Clear();

            var groupsQuery = Forms.Search.DeclaredEvents.AsQueryable().Where(x => x.Visible);

            var selectedGroup = EventGroupSelector.Value;
            if (!string.IsNullOrEmpty(selectedGroup))
                groupsQuery = groupsQuery.Where(x => x.Group == selectedGroup);

            var groups = groupsQuery.GroupBy(x => x.Group).OrderBy(x => x.Key).ToArray();

            if (groups.Length != 1)
            {
                foreach (var group in groups)
                {
                    var optionGroup = new ComboBoxOptionGroup(group.Key);
                    foreach (var item in group)
                        optionGroup.Items.Add(new ComboBoxOption(item.Name, item.Type.FullName));

                    EventNameSelector.Items.Add(optionGroup);
                }
            }
            else
            {
                foreach (var item in groups[0])
                    EventNameSelector.Items.Add(new ComboBoxOption(item.Name, item.Type.FullName));
            }
        }

        #endregion

        #region Methods

        public override void Clear()
        {
            EventGroupSelector.ClearSelection();
            OnEventGroupChanged();
            EventNameSelector.ClearSelection();
            UserSelector.ClearSelection();

            UserEmail.Text = null;

            RecordTimeSince.Value = null;
            RecordTimeBefore.Value = null;
        }

        #endregion
    }
}