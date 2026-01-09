using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using ListItem = System.Web.UI.WebControls.ListItem;

namespace InSite.UI.Admin.Events.Classes
{
    public partial class ChangePrivacy : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={EventId}&panel=privacy";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventId}&panel=privacy" : null;

        private Guid EventId => Guid.TryParse(Request["event"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterGroupType.AutoPostBack = true;
            FilterGroupType.ValueChanged += (x, y) => ApplyFilter();
            FilterGroupButton.Click += (x, y) => ApplyFilter();

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            Open();
        }

        private void Open()
        {
            var @event = ServiceLocator.EventSearch.GetEvent(EventId);
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            CancelButton.NavigateUrl = OutlineUrl;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            ClassSummaryInfo.Bind(@event);

            FilterGroupType.Value = GroupTypes.Role;
            ApplyFilter();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            SaveGroups();

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        public void SaveGroups()
        {
            var grants = new List<Guid>();
            var revokes = new List<Guid>();

            foreach (ListItem item in FilterGroupList.Items)
            {
                var group = Guid.Parse(item.Value);
                if (item.Selected)
                    grants.Add(group);
                else
                    revokes.Add(group);
            }

            TGroupPermissionStore.Update(DateTimeOffset.UtcNow, User.UserIdentifier, EventId, "Event", grants, revokes);
        }

        private void ApplyFilter()
        {
            var filter = GetFilter();
            var groups = ServiceLocator.GroupSearch.GetGroups(filter);

            FilterGroupList.Items.Clear();

            foreach (var group in groups)
            {
                var isExists = TGroupPermissionSearch.Exists(x => x.GroupIdentifier == group.GroupIdentifier && x.ObjectIdentifier == EventId);

                FilterGroupList.Items.Add(new ListItem
                {
                    Value = group.GroupIdentifier.ToString(),
                    Text = group.GroupName,
                    Selected = isExists
                });
            }
        }

        private QGroupFilter GetFilter()
        {
            var filterGroupName = FilterGroupName.Text;

            var organizations = new List<Guid> { Organization.Identifier };
            if (Organization.ParentOrganizationIdentifier.HasValue)
                organizations.Add(Organization.ParentOrganizationIdentifier.Value);

            return new QGroupFilter
            {
                OrganizationIdentifiers = organizations.ToArray(),
                GroupType = FilterGroupType.Value,
                GroupNameLike = filterGroupName
            };
        }
    }
}